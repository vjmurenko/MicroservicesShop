﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OrkestratorService.Database;

#nullable disable

namespace OrkestratorService.Migrations
{
    [DbContext(typeof(OrderStateDbContext))]
    partial class OrderStateDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("OrkestratorService.Database.Models.CartItem", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<int>("Amount")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uuid");

                    b.Property<int>("Price")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("OrderId");

                    b.ToTable("CartItems");
                });

            modelBuilder.Entity("OrkestratorService.Sagas.OrderSaga.OrderState", b =>
                {
                    b.Property<Guid>("CorrelationId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset?>("ConfirmDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CurrentState")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("FeedbackTimeoutToken")
                        .HasColumnType("uuid");

                    b.Property<string>("Manager")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset?>("RejectDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("RejectionReason")
                        .HasColumnType("text");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("bytea");

                    b.Property<DateTimeOffset>("SubmitDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("TotalPrice")
                        .HasColumnType("integer");

                    b.HasKey("CorrelationId");

                    b.HasIndex("CorrelationId")
                        .IsUnique();

                    b.ToTable("OrderStates");
                });

            modelBuilder.Entity("OrkestratorService.Database.Models.CartItem", b =>
                {
                    b.HasOne("OrkestratorService.Sagas.OrderSaga.OrderState", "OrderState")
                        .WithMany("CartItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OrderState");
                });

            modelBuilder.Entity("OrkestratorService.Sagas.OrderSaga.OrderState", b =>
                {
                    b.Navigation("CartItems");
                });
#pragma warning restore 612, 618
        }
    }
}
