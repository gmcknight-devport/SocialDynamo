using Microsoft.EntityFrameworkCore;
using Posts.Domain.Models;
using Posts.Domain.ValueObjects;

namespace Posts.Infrastructure.Persistence
{
    public class PostsDbContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }

        public PostsDbContext(DbContextOptions<PostsDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                .Property(p => p.PostId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Comment>()
                .Property(c => c.CommentId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<CommentLike>()
                .HasOne(c => c.Comment)
                .WithMany(l => l.Likes);

            modelBuilder.Entity<PostLike>()
                .HasOne(p => p.Post)
                .WithMany(l => l.Likes);
        }
    }
}
