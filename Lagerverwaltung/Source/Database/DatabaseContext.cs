using Microsoft.EntityFrameworkCore;
using Lagerverwaltung.Database;

namespace Lagerverwaltung.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<DeliveryCompany> DeliveryCompanies { get; set; }
        public DbSet<Reorder> Reorders { get; set; }
        public DbSet<Sale> Sales { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product>()
                .HasIndex(u => u.Name)
                .IsUnique();
        }
    }
}
