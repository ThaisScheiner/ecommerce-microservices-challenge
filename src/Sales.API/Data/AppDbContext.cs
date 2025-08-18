using Microsoft.EntityFrameworkCore;
using Sales.API.Entities;

namespace Sales.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
    }
}