using Microsoft.EntityFrameworkCore;
using RasyonetTracker.Models;

namespace RasyonetTracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Stock> Stocks { get; set; }
    }
}