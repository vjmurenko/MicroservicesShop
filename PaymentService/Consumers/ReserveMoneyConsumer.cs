using MassTransit;
using Microsoft.Extensions.Logging;
using Moneyreservation.Contracts;

namespace PaymentService.Consumers;

public class ReserveMoneyConsumer : IConsumer<ReserveMoney>
{
    private readonly ILogger<ReserveMoneyConsumer> _logger;

    public ReserveMoneyConsumer(ILogger<ReserveMoneyConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ReserveMoney> context)
    {
        _logger.LogInformation($"ReserveMoney for {context.Message.Amount} $ ");
        await context.RespondAsync<MoneyReserved>(new
        {
            OrderId = context.Message.OrderId,
            Amount = context.Message.Amount
        });
    }
}