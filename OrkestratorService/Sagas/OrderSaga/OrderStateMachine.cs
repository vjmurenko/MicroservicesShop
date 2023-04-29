using Api.Contracts.ManagerApi;
using Api.Contracts.UserApi;
using Automatonymous;
using Automatonymous.Binders;
using CartService.Contracts;
using DeliveryService.Contracts;
using FeedbkackService.Contracts;
using HistoryService.Contract;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moneyreservation.Contracts;
using OrkestratorService.Confgirations;
using OrkestratorService.Database.Converter;

namespace OrkestratorService.Sagas.OrderSaga;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    private readonly EndpointsConfiguration _endpointsConfiguration;
    private readonly ILogger<OrderStateMachine> _logger;

    public State AwaitingConfirmation { get; set; }
    public State AwaitingDelivery { get; set; }
    public State AwaitingFeedback { get; set; }

    public Request<OrderState, GetCart, GetCartResponse> CartRequest { get; set; }
    public Request<OrderState, ReserveMoney, MoneyReserved> MoneyReservationRequest { get; set; }
    public Request<OrderState, UnreserveMoney, MoneyUnreserved> MoneyUnreservationRequest { get; set; }
    public Request<OrderState, StoreFeedback, FeedbackSaved> FeedbackRequest { get; set; }
    public Request<OrderState, ArchiveOrder, OrderArchived> ArchiveRequest { get; set; }

    public Schedule<OrderState, FeedbackTimeout> OrderFeedbackTimeout { get; set; }

    public Event<SubmitOrder> SubmitOrder { get; set; }
    public Event<ConfirmOrder> ConfirmOrder { get; set; }
    public Event<RejectOrder> RejectOrder { get; set; }
    public Event<OrderDelivered> OrderDelivered { get; set; }
    public Event<SendFeedback> SendFeedback { get; set; }


    public OrderStateMachine(EndpointsConfiguration endpointsConfiguration, ILogger<OrderStateMachine> logger)
    {
        _endpointsConfiguration = endpointsConfiguration;
        _logger = logger;

        BuildStateMachine();

        InstanceState(state => state.CurrentState);
        
        Initially(WhenOrderSubmitted());
        During(CartRequest.Pending, WhenCartReturned());
        During(MoneyReservationRequest.Pending, WhenMoneyReserved());
        During(AwaitingConfirmation, WhenOrderConfirmed(), WhenOrderRejected());
        During(MoneyUnreservationRequest.Pending, WhenMoneyReturned());
        During(AwaitingDelivery, WhenOrderDelivered());
        During(AwaitingFeedback, WhenReceivedFeedback(), WhenIimeOutFeedback());
        During(FeedbackRequest.Pending, WhenFeedbackReceived());
        During(ArchiveRequest.Pending, WhenOrderArchived());

        SetCompletedWhenFinalized();
    }

    private void BuildStateMachine()
    {
        Event(() => SubmitOrder, s => s.CorrelateById(context => context.Message.OrderId));
        Event(() => ConfirmOrder, s => s.CorrelateById(context => context.Message.OrderId));
        Event(() => RejectOrder, s => s.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderDelivered, s => s.CorrelateById(context => context.Message.OrderId));
        Event(() => SendFeedback, s => s.CorrelateById(context => context.Message.OrderId));

        Request(() => CartRequest, r =>
        {
            r.ServiceAddress = new Uri(_endpointsConfiguration.CartServiceAddress);
            r.Timeout = TimeSpan.Zero;
        });
        
        Request(() => MoneyReservationRequest, r =>
        {
            r.ServiceAddress = new Uri(_endpointsConfiguration.PaymentServiceAddress);
            r.Timeout = TimeSpan.Zero;
        });
        
        Request(() => MoneyUnreservationRequest, r =>
        {
            r.ServiceAddress = new Uri(_endpointsConfiguration.PaymentServiceAddress);
            r.Timeout = TimeSpan.Zero;
        });
        
        Request(() => FeedbackRequest, r =>
        {
            r.ServiceAddress = new Uri(_endpointsConfiguration.FeedbackServiceAddress);
            r.Timeout = TimeSpan.Zero;
        });
        
        Request(() => ArchiveRequest, r =>
        {
            r.ServiceAddress = new Uri(_endpointsConfiguration.HistoryServiceAddress);
            r.Timeout = TimeSpan.Zero;
        });

        Schedule(() => OrderFeedbackTimeout, instance => instance.FeedbackTimeoutToken, s =>
        {
            s.Delay = TimeSpan.FromMinutes(1);
            s.Received = r => r.CorrelateById(c => c.Message.OrderId);
        });

    }

    private EventActivities<OrderState> WhenOrderSubmitted()
    {
        return When(SubmitOrder)
            .Request(CartRequest, context => context.Init<GetCart>(new
            {
                Id = context.CorrelationId
            }))
            .TransitionTo(CartRequest.Pending);
    }

    private EventActivities<OrderState> WhenCartReturned()
    {
        return When(CartRequest.Completed)
            .Then(context =>
            {
                context.Instance.CartItems = DbConverter.ConvertToCartItemsList(context.Data.CartContent, context.Data.OrderId);
                context.Instance.TotalPrice = context.Data.TotalPrice;
                context.Instance.SubmitDate = DateTimeOffset.Now;
            })
            .Request(MoneyReservationRequest, context => context.Init<ReserveMoney>(new
            {
                OrderId = context.Instance.CorrelationId,
                Amount = context.Instance.TotalPrice
            }))
            .TransitionTo(MoneyReservationRequest.Pending);
    }

    private EventActivities<OrderState> WhenMoneyReserved()
    {
        return When(MoneyReservationRequest.Completed)
            .TransitionTo(AwaitingConfirmation);
    }

    private EventActivities<OrderState> WhenOrderConfirmed()
    {
        return When(ConfirmOrder)
            .Then(context =>
            {
                context.Instance.Manager = context.Data.Manager;
                context.Instance.ConfirmDate = DateTimeOffset.Now;
            })
            .SendAsync(new Uri(_endpointsConfiguration.DeliveryServiceAddress), context => context.Init<DeliverOrder>(new
            {
                OrderId = context.Instance.CorrelationId
            }))
            .TransitionTo(AwaitingDelivery);
    }

    private EventActivities<OrderState> WhenOrderRejected()
    {
        return When(RejectOrder)
            .Then(context =>
            {
                context.Instance.Manager = context.Data.Manager;
                context.Instance.RejectDate = DateTimeOffset.Now;
                context.Instance.RejectionReason = context.Data.RejectionReason;
            })
            .Request(MoneyUnreservationRequest, context => context.Init<UnreserveMoney>(new
            {
                OrderId = context.Instance.CorrelationId,
                Amount = context.Instance.TotalPrice
            }))
            .TransitionTo(MoneyUnreservationRequest.Pending);
    }

    private EventActivities<OrderState> WhenMoneyReturned()
    {
        return When(MoneyUnreservationRequest.Completed)
            .Request(ArchiveRequest, context => context.Init<ArchiveOrder>(new
            {
                context.Instance.CorrelationId,
                context.Instance.TotalPrice,
                context.Instance.SubmitDate,
                CurrentState = MoneyUnreservationRequest.Completed.Name,
                context.Instance.Manager,
                context.Instance.RejectDate,
                context.Instance.RejectionReason,
                context.Instance.ConfirmDate
            }))
            .TransitionTo(ArchiveRequest.Pending);
    }

    private EventActivities<OrderState> WhenOrderDelivered()
    {
        return When(OrderDelivered)
            .Schedule(OrderFeedbackTimeout, s => s.Init<FeedbackTimeout>(new
            {
                OrderId = s.Instance.CorrelationId
            }))
            .TransitionTo(AwaitingFeedback);
    }

    private EventActivities<OrderState> WhenReceivedFeedback()
    {
        return When(SendFeedback)
            .Unschedule(OrderFeedbackTimeout)
            .Request(FeedbackRequest, context => context.Init<StoreFeedback>(new
            {
                context.Data.OrderId,
                context.Data.Stars,
                context.Data.Message
            }))
            .TransitionTo(FeedbackRequest.Pending);
    }

    private EventActivities<OrderState> WhenIimeOutFeedback()
    {
        return When(OrderFeedbackTimeout.Received)
            .Then(c => _logger.LogInformation("Feedback timeout"))
            .Request(ArchiveRequest, context => context.Init<ArchiveOrder>(new
            {
                context.Instance.CorrelationId,
                context.Instance.TotalPrice,
                context.Instance.SubmitDate,
                context.Instance.Manager,
                context.Instance.RejectDate,
                context.Instance.RejectionReason,
                context.Instance.ConfirmDate,
                CurrentState = OrderFeedbackTimeout.Received.Name
            }))
            .TransitionTo(ArchiveRequest.Pending);
    }

    private EventActivities<OrderState> WhenFeedbackReceived()
    {
        return When(FeedbackRequest.Completed)
            .Request(ArchiveRequest, context => context.Init<ArchiveOrder>(new
            {
                context.Instance.CorrelationId,
                context.Instance.TotalPrice,
                context.Instance.SubmitDate,
                context.Instance.Manager,
                context.Instance.RejectDate,
                context.Instance.RejectionReason,
                context.Instance.ConfirmDate,
                CurrentState = FeedbackRequest.Completed.Name
            }))
            .TransitionTo(ArchiveRequest.Pending);
    }

    private EventActivities<OrderState> WhenOrderArchived()
    {
        return When(ArchiveRequest.Completed)
            .Finalize();
    }
}