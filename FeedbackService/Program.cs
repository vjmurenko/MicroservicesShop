
using FeedbackService.Consumers;
using FeedbackService.Database;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrkestratorService.Confgirations;

public class Program
{
    public static void Main(string[] args)
    {
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var endppointsConfig = hostContext.Configuration.GetSection("EndpointsConfiguration").Get<EndpointsConfiguration>();
                var rabbitConfig = hostContext.Configuration.GetSection("RabbitmqConfiguration").Get<RabbitmqConfiguration>();
                
                var contextOptions = new DbContextOptionsBuilder()
                    .UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection"))
                    .Options;
                
                using var context = new FeedbackDbContext(contextOptions);
                context.Database.Migrate();

                services.AddDbContext<FeedbackDbContext>(s => s.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")),
                    ServiceLifetime.Transient, ServiceLifetime.Transient);

                services.AddMassTransit(cfg =>
                {
                    cfg.AddConsumer<StoreFeedbackConsumer>(typeof(StoreFeedbackDefinition))
                        .Endpoint(e => e.Name = endppointsConfig.FeedbackServiceAddress);
                    
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