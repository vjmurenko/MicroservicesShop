using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace CartService.Consumers;

public class AddCartPositionConsumerDefinition : ConsumerDefinition<AddCartPositionConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AddCartPositionConsumer> consumerConfigurator)
    {
        consumerConfigurator.UseDelayedRedelivery(r => r.Immediate(5));
        consumerConfigurator.UseMessageRetry(r => r.Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
        consumerConfigurator.UseInMemoryOutbox();
    }
}