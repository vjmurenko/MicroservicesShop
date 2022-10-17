using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace CartService.Consumers;

public class GetCartConsumerDefinition : ConsumerDefinition<GetCartConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<GetCartConsumer> consumerConfigurator)
    {
        consumerConfigurator.UseDelayedRedelivery(r => r.Immediate(5));
        consumerConfigurator.UseMessageRetry(r => r.Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
        consumerConfigurator.UseInMemoryOutbox();
    }
}