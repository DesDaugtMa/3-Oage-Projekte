using Microsoft.EntityFrameworkCore;
using Lagerverwaltung.Database;

namespace Lagerverwaltung.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product>()
                .HasIndex(u => u.Name)
                .IsUnique();
        }
        public DbSet<Lagerverwaltung.Database.Category> Category { get; set; } = default!;
        public DbSet<Lagerverwaltung.Database.Sale> Sale { get; set; } = default!;
    }
}
