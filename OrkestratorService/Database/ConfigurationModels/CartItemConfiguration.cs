using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrkestratorService.Database.Models;

namespace OrkestratorService.Database.ConfigurationModels;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.Id).IsUnique();
        
        builder.Property(s => s.Id).IsRequired().ValueGeneratedNever();
    }
}