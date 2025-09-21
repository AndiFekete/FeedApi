using FeedApi.Data.Entities;
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
    public DbSet<ImageFeed> ImageFeeds { get; set; }
    public DbSet<VideoFeed> VideoFeeds { get; set; }
} 