using GreenPipes;
using MassTransit;
using MassTransit.Definition;

namespace OrkestratorService.Sagas.OrderSaga;

public class OrderStateMahineDefintion : SagaDefinition<OrderState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator)
    {
        endpointConfigurator.UseMessageRetry(configurator => configurator.Intervals(100, 200, 300, 500, 1000));
        endpointConfigurator.UseInMemoryOutbox();
    }
}