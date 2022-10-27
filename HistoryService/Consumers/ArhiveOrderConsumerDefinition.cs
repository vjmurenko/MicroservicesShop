using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace HistoryService.Consumers;

public class ArhiveOrderConsumerDefinition : ConsumerDefinition<ArhiveOrderConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ArhiveOrderConsumer> consumerConfigurator)
    {
        consumerConfigurator.UseDelayedRedelivery(r => r.Immediate(5));
        consumerConfigurator.UseMessageRetry(r => r.Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
        consumerConfigurator.UseInMemoryOutbox();
    }
}