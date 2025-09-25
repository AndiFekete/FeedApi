using FeedsApi.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

        modelBuilder.Entity<Feed>().HasData(
            new Feed
            {
                FeedId = 1,
                UserId = 1,
                Title = "Text Feed",
                Description = "This is a text feed.",
                Discriminator = "Text"
            }
        );

        modelBuilder.Entity<ImageFeed>().HasData(
            new ImageFeed
            {
                FeedId = 2,
                UserId = 2,
                Title = "Image Feed",
                Description = "This is an image feed.",
                Discriminator = "Image",
                ImageData = new byte[] { 1, 2, 3 }
            }
        );

        modelBuilder.Entity<VideoFeed>().HasData(
            new VideoFeed
            {
                FeedId = 3,
                UserId = 3,
                Title = "Video Feed",
                Description = "This is a video feed.",
                Discriminator = "Video",
                Url = "https://example.com/video.mp4"
            }
        );

        modelBuilder.Entity<Like>().HasData(
            new Like
            {
                FeedId = 1,
                UserId = 1
            },
            new Like
            {
                FeedId = 2,
                UserId = 1
            }
        );

        modelBuilder.Entity<Comment>().HasData(
            new Comment
            {
                Id = 1,
                FeedId = 1,
                UserId = 1,
                Text = "Wow!"
            },
            new Comment
            {
                Id = 2,
                FeedId = 3,
                UserId = 1,
                Text = "Such comment."
            }
        );
    }
}