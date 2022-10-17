using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrkestratorService.Sagas.OrderSaga;

namespace OrkestratorService.Database.ConfigurationModels;

public class SagaClassMap : SagaClassMap<OrderState>
{
    protected override void Configure(EntityTypeBuilder<OrderState> entity, ModelBuilder model)
    {
        entity.HasKey(s => s.CorrelationId);
        entity.HasIndex(s => s.CorrelationId).IsUnique();

        entity.HasMany(s => s.CartItems)
            .WithOne(s => s.OrderState)
            .HasForeignKey(s => s.OrderId);
        
        entity.Property(o => o.RowVersion).IsRowVersion();
    }
}