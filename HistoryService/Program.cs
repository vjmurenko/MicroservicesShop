using HistoryService.Consumers;
using HistoryService.Database;
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
                // var contextOptions = new DbContextOptionsBuilder().UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")).Options;
                // using var context = new NpgSqlContext(contextOptions);
                // context.Database.Migrate();
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                
                var contextOptions = new DbContextOptionsBuilder()
                    .UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection"))
                    .Options;
                
                using var context = new HistoryDbContext(contextOptions);
                context.Database.Migrate();

                services.AddDbContext<HistoryDbContext>(opt => { opt.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")); },
                    ServiceLifetime.Transient, ServiceLifetime.Transient);

                var endpointsConfig = hostContext.Configuration.GetSection("EndpointsConfiguration").Get<EndpointsConfiguration>();

                var rabbitmqConfig = hostContext.Configuration.GetSection("RabbitmqConfiguration").Get<RabbitmqConfiguration>();

                services.AddMassTransit(x =>
                {
                    x.AddConsumer<ArhiveOrderConsumer>(typeof(ArhiveOrderConsumerDefinition))
                        .Endpoint(cfg => { cfg.Name = endpointsConfig.HistoryServiceAddress; });

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.UseBsonSerializer();
                        cfg.ConfigureEndpoints(context);
                        cfg.Host(rabbitmqConfig.Hostname, rabbitmqConfig.VirtualHost, h =>
                        {
                            h.Username(rabbitmqConfig.Username);
                            h.Password(rabbitmqConfig.Password);
                        });
                    });
                }).AddMassTransitHostedService(true);
                
            }).Build().Run();
    }
}