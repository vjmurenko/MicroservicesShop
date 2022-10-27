using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using OrkestratorService.Database.ConfigurationModels;
using OrkestratorService.Database.Models;
using OrkestratorService.Sagas.OrderSaga;

namespace OrkestratorService.Database;

public class OrderStateDbContext : SagaDbContext
{
    public DbSet<OrderState> OrderStates { get; set; }
    public DbSet<CartItem> CartItems { get; set; }


    public OrderStateDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CartItemConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new SagaClassMap(); }
    }
}