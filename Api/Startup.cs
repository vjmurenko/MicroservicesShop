using Api.Configurations;
using Api.Models.Implementations;
using Api.Models.Interfaces;
using MassTransit;
using Microsoft.OpenApi.Models;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Api", Version = "v1"}); });
            
            var routingSection = Configuration.GetSection("RoutingConfiguration");
            var routingConfig = routingSection.Get<RoutingConfiguration>();
            services.AddTransient<IRoutingConfiguration>(services => routingConfig);
            
            var rabbitMqConfig = Configuration.GetSection("RabbitMqConfiguration").Get<RabbitMqConfiguration>();

            services.AddMassTransit(cfg =>
            {
                cfg.UsingRabbitMq((context, cfg) =>
                {
                    cfg.UseBsonSerializer();
                    cfg.ConfigureEndpoints(context);
                    cfg.Host(rabbitMqConfig.Hostname, rabbitMqConfig.VirtualHost, c =>
                    {
                        c.Username(rabbitMqConfig.Username);
                        c.Password(rabbitMqConfig.Password);
                    });
                });
            }).AddMassTransitHostedService(true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}