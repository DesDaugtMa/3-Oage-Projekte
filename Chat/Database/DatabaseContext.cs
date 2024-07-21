using Microsoft.EntityFrameworkCore;

namespace Chat.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Like>().HasKey(x => new { x.MessageId, x.UserId });

            modelBuilder.Entity<Like>()
                .HasOne(x => x.User)
                .WithMany(x => x.Likes)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Like>()
                .HasOne(x => x.Message)
                .WithMany(x => x.Likes)
                .HasForeignKey(x => x.MessageId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
