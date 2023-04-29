using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrkestratorService.Confgirations;
using OrkestratorService.Database;
using OrkestratorService.Sagas.OrderSaga;

namespace OrkestratorService;

public class Program
{
    public static void Main(string[] args)
    {
        Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) =>
            {
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                var connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");

                services.AddDbContext<OrderStateDbContext>(opt => opt.UseNpgsql(connectionString), ServiceLifetime.Transient, ServiceLifetime.Transient);
                
                var contextOptions = new DbContextOptionsBuilder()
                    .UseNpgsql(connectionString)
                    .Options;
                
                using var context = new OrderStateDbContext(contextOptions);
                context.Database.Migrate();

                var endpointsConfig = hostContext.Configuration.GetSection("EndpointsConfiguration").Get<EndpointsConfiguration>();
                services.AddTransient(s => endpointsConfig);
                
                var rabbitConfiguration = hostContext.Configuration.GetSection("RabbitMqConfiguration").Get<RabbitmqConfiguration>();

                services.AddMassTransit(configurator =>
                {
                    configurator.AddSagaRepository<OrderState>()
                        .EntityFrameworkRepository(r =>
                        {
                            r.ExistingDbContext<OrderStateDbContext>();
                            r.ConcurrencyMode = ConcurrencyMode.Optimistic;
                        });
                    
                    configurator.AddSagaStateMachine<OrderStateMachine, OrderState>(typeof(OrderStateMahineDefintion))
                        .Endpoint(e => e.Name = endpointsConfig.OrderStateMachineAddress);

                    configurator.AddDelayedMessageScheduler();

                    configurator.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                        cfg.UseBsonSerializer();
                        cfg.UseDelayedMessageScheduler();
                        cfg.Host(rabbitConfiguration.Hostname, rabbitConfiguration.VirtualHost, host =>
                        {
                            host.Username(rabbitConfiguration.Username);
                            host.Password(rabbitConfiguration.Password);
                        });
                    });
                }).AddMassTransitHostedService(true);

            })
            .Build().Run();
    }
}