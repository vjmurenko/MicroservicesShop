using DeliveryService.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DeliveryService.Consumers;

public class DeliverOrderConsumer : IConsumer<DeliverOrder>
{
    private readonly ILogger<DeliverOrderConsumer> _logger;

    public DeliverOrderConsumer(ILogger<DeliverOrderConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DeliverOrder> context)
    {
        _logger.LogInformation("Order delivered");
        
        await Task.Delay(10000);

        await context.Publish<OrderDelivered>(new
        {
            context.Message.OrderId
        });
    }
}