using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace DeliveryService.Consumers;

public class DeliverOrderConsumerDefinition : ConsumerDefinition<DeliverOrderConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DeliverOrderConsumer> consumerConfigurator)
    {
        consumerConfigurator.UseDelayedRedelivery(s => s.Immediate(5));
        consumerConfigurator.UseMessageRetry(r => r.Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
        consumerConfigurator.UseInMemoryOutbox();
    }
}