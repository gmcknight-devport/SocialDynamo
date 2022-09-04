using Account.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Persistence
{
    public class AccountDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Follower> Followers { get; set; }

        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(p => p.UserId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Follower>()
                .HasOne(u => u.User)
                .WithMany(f => f.Followers)
                .HasForeignKey(u => u.UserId)
                .HasForeignKey(u => u.FollowerId);
        }
    }
}
