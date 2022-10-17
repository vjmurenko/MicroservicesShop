using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OrkestratorService.Confgirations;
using PaymentService.Consumers;

public class Program
{
    public static void Main(string[] args)
    {
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var rabbitmqConfig = hostContext.Configuration.GetSection("RabbitMqConfiguration").Get<RabbitmqConfiguration>();
                var endpointConfig = hostContext.Configuration.GetSection("EndpointsConfiguration").Get<EndpointsConfiguration>();
                services.AddMassTransit(config =>
                {
                    config.AddConsumer<ReserveMoneyConsumer>(typeof(ReserveMoneyConsumerDefinition))
                        .Endpoint(e => e.Name = endpointConfig.PaymentServiceAddress);

                    config.AddConsumer<UnreserveMoneyConsumer>()
                        .Endpoint(e => e.Name = endpointConfig.PaymentServiceAddress);

                    config.UsingRabbitMq((context, conf) =>
                    {
                        conf.ConfigureEndpoints(context);
                        conf.UseBsonSerializer();
                        conf.Host(rabbitmqConfig.Hostname, rabbitmqConfig.VirtualHost, h =>
                        {
                            h.Username(rabbitmqConfig.Username);
                            h.Password(rabbitmqConfig.Password);
                        });
                    });
                }).AddMassTransitHostedService(true);
            }).Build().Run();
    }
}