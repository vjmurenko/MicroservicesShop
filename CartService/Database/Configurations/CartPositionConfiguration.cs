﻿using CartService.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CartService.Database.Configurations;

public class CartPositionConfiguration : IEntityTypeConfiguration<CartPosition>
{
    public void Configure(EntityTypeBuilder<CartPosition> builder)
    {
        builder.HasKey(cp => cp.Id);
        builder.HasIndex(cp => cp.Id).IsUnique();

        builder.HasOne(cp => cp.Good);

        builder.Property(cp => cp.Amount).IsRequired();
        builder.Property(cp => cp.CartId).IsRequired();
        builder.Property(cp => cp.GoodId).IsRequired();
    }
}