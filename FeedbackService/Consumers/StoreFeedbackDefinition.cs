using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace FeedbackService.Consumers;

public class StoreFeedbackDefinition : ConsumerDefinition<StoreFeedbackConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<StoreFeedbackConsumer> consumerConfigurator)
    {
        consumerConfigurator.UseDelayedRedelivery(s => s.Immediate(5));
        consumerConfigurator.UseMessageRetry(r => r.Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
        consumerConfigurator.UseInMemoryOutbox();
    }
}