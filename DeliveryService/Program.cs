using DeliveryService.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OrkestratorService.Confgirations;

namespace DeliveryService;

public class Program
{
    public static void Main(string[] args)
    {
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var endppointsConfig = hostContext.Configuration.GetSection("EndpointsConfiguration").Get<EndpointsConfiguration>();
                var rabbitConfig = hostContext.Configuration.GetSection("RabbitmqConfiguration").Get<RabbitmqConfiguration>();
                
                services.AddMassTransit(cfg =>
                {
                    cfg.AddConsumer<DeliverOrderConsumer>(typeof(DeliverOrderConsumerDefinition))
                        .Endpoint(e => e.Name = endppointsConfig.DeliveryServiceAddress);
                    
                    cfg.UsingRabbitMq((context, config) =>
                    {
                        config.UseBsonSerializer();
                        config.ConfigureEndpoints(context);
                        config.Host(rabbitConfig.Hostname, rabbitConfig.VirtualHost, h =>
                        {
                            h.Username(rabbitConfig.Username);
                            h.Password(rabbitConfig.Password);
                        });
                        
                    });
                }).AddMassTransitHostedService(true);

            }).Build().Run();
    }
}