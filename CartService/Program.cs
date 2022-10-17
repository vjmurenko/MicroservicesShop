using CartService.Configurations;
using CartService.Consumers;
using CartService.Database;
using CartService.Database.Repository;
using CartService.Database.Repository.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CartService;

public class Program
{
    public static void Main(string[] args)
    {
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var contextOptions = new DbContextOptionsBuilder().UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")).Options;
                using var context = new NpgSqlContext(contextOptions);
                context.Database.Migrate();

                services.AddTransient<ICartRepository, CartRepository>();
                services.AddTransient<ICartPositionRepository, CartPositionRepository>();
                services.AddTransient<IGoodRepository, GoodRepository>();

                services.AddDbContext<NpgSqlContext>(opt => { opt.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")); },
                    ServiceLifetime.Transient, ServiceLifetime.Transient);

                var endpointsConfig = hostContext.Configuration.GetSection("EndpointsConfiguration").Get<EndpointsConfiguration>();

                var rabbitmqConfig = hostContext.Configuration.GetSection("RabbitmqConfiguration").Get<RabbitmqConfiguration>();

                services.AddMassTransit(x =>
                {
                    x.AddConsumer<AddCartPositionConsumer>(typeof(AddCartPositionConsumerDefinition))
                        .Endpoint(cfg => { cfg.Name = endpointsConfig.CartServiceAddress; });

                    x.AddConsumer<GetCartConsumer>(typeof(GetCartConsumerDefinition))
                        .Endpoint(cfg => { cfg.Name = endpointsConfig.CartServiceAddress; });
                    
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