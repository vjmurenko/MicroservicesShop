using MassTransit;
using Microsoft.Extensions.Logging;
using Moneyreservation.Contracts;

namespace PaymentService.Consumers;

public class UnreserveMoneyConsumer : IConsumer<UnreserveMoney>
{
    private readonly ILogger<UnreserveMoneyConsumer> _logger;

    public UnreserveMoneyConsumer(ILogger<UnreserveMoneyConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UnreserveMoney> context)
    {
        _logger.LogInformation($"Unreserve money for {context.Message.Amount} $ ");
        await context.RespondAsync<MoneyUnreserved>(new
        {
            context.Message.OrderId, context.Message.Amount
        });
    }
}