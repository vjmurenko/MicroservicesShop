using HistoryService.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace HistoryService.Database;

public class HistoryDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }

    public HistoryDbContext(DbContextOptions options) : base(options)
    {
        
    }
}