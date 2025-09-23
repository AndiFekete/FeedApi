using FeedApi.Data.Entities;
using FeedsApi.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;

public class FeedDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public FeedDbContext(DbContextOptions<FeedDbContext> options)
        : base(options)
    {
    }

    public DbSet<Feed> Feeds { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Feed>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<Feed>("Text")
            .HasValue<ImageFeed>("Image")
            .HasValue<VideoFeed>("Video");
    }
} 