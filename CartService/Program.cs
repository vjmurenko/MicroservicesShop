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
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var contextOptions = new DbContextOptionsBuilder().UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")).Options;
                using var context = new NpgSqlContext(contextOptions);
                context.Database.Migrate();

                services.AddTransient<ICartRepository, CartRepository>();
                services.AddTransient<ICartPositionRepository, CartPositionRepository>();
                services.AddTransient<IGoodRepository, GoodRepository>();

                services.AddDbContext<NpgSqlContext>(opt =>
                {
                    opt.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection"));
                }, ServiceLifetime.Transient, ServiceLifetime.Transient);

                var endpointsSection = hostContext.Configuration.GetSection("EndpointsConfiguration");
                var endpointsConfig = endpointsSection.Get<EndpointsConfiguration>();

                var rabbitmqSection = hostContext.Configuration.GetSection("RabbitmqConfiguration");
                var rabbitmqConfig = rabbitmqSection.Get<RabbitmqConfiguration>();

                services.AddMassTransit(x =>
                {
                    x.AddConsumer<AddCartPositionConsumer>(typeof(AddCartPositionConsumerDefinition))
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
            });
    }
}