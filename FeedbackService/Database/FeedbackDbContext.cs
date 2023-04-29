using FeedbackService.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace FeedbackService.Database;

public class FeedbackDbContext : DbContext
{
    public DbSet<Feedback> Feebacks { get; set; }

    public FeedbackDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Feedback>()
            .HasKey(s => s.OrderId);
    }
}