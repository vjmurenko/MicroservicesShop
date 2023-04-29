using GreenPipes;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using MassTransit;

namespace PaymentService.Consumers;

public class UnreserveMoneyConsumerDefinition : ConsumerDefinition<UnreserveMoneyConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<UnreserveMoneyConsumer> consumerConfigurator)
    {
        consumerConfigurator.UseDelayedRedelivery(s => s.Immediate(5));
        consumerConfigurator.UseMessageRetry(r => r.Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
        consumerConfigurator.UseInMemoryOutbox();
    }
}
