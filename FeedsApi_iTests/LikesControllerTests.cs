using FeedsApi.Controllers;
using FeedsApi.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Security.Principal;

namespace FeedsApi_iTests
{
    [TestFixture]
    public class LikesControllerTests
    {
        private FeedDbContext _context;

        [SetUp]
        public void Setup()
        {
            _context = GetDbContext();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetLikingUsers_ReturnsUsers()
        {
            _context.Feeds.Add(new Feed { FeedId = 1, Title = "test", Description = "desc", UserId = 99, Discriminator = "Text" });
            _context.Likes.Add(new Like { FeedId = 1, UserId = 42 });
            _context.Likes.Add(new Like { FeedId = 1, UserId = 77 });
            await _context.SaveChangesAsync();

            var controller = GetController(_context);

            var result = await controller.GetLikingUsers(1);

            var ok = result.Result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);

            var users = ok.Value as List<int>;
            Assert.That(users, Is.Not.Null);
            Assert.That(2, Is.EqualTo(users.Count));
            Assert.That(users, Contains.Item(42));
            Assert.That(users, Contains.Item(77));
        }

        [Test]
        public async Task LikeFeed_ReturnsCreated_WhenFeedExists()
        {
            _context.Feeds.Add(new Feed { FeedId = 1, Title = "t", Description = "d", UserId = 99, Discriminator = "Text" });
            await _context.SaveChangesAsync();

            var controller = GetController(_context);

            var result = await controller.LikeFeed(1);

            Assert.That(result, Is.TypeOf<CreatedResult>());
            Assert.That(_context.Likes.Count(), Is.EqualTo(1));
            Assert.That(_context.Likes.First().UserId, Is.EqualTo(42));
        }

        [Test]
        public async Task LikeFeed_ReturnsBadRequest_WhenFeedDoesNotExist()
        {
            var controller = GetController(_context);

            var result = await controller.LikeFeed(123);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            var badRequest = result as BadRequestObjectResult;
            Assert.That(badRequest.Value, Is.EqualTo("Cannot like non-existent feed"));
        }

        [Test]
        public async Task UnlikeFeed_ReturnsNoContent_WhenLikeExists()
        {
            _context.Feeds.Add(new Feed { FeedId = 1, Title = "t", Description = "d", UserId = 99, Discriminator = "Text" });
            _context.Likes.Add(new Like { FeedId = 1, UserId = 42 });
            await _context.SaveChangesAsync();

            var controller = GetController(_context);

            var result = await controller.UnlikeFeed(1);

            Assert.That(result, Is.TypeOf<NoContentResult>());
            Assert.That(_context.Likes, Is.Empty);
        }

        [Test]
        public async Task UnlikeFeed_ReturnsNotFound_WhenLikeDoesNotExist()
        {
            _context.Feeds.Add(new Feed { FeedId = 1, Title = "t", Description = "d", UserId = 99, Discriminator = "Text" });
            await _context.SaveChangesAsync();

            var controller = GetController(_context);

            var result = await controller.UnlikeFeed(1);

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }


        private FeedDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<FeedDbContext>()
                .UseInMemoryDatabase("LikesControllerDb")
                .Options;
            return new FeedDbContext(options);
        }

        private LikesController GetController(FeedDbContext context, int userId = 42)
        {
            var loggerMock = new Mock<ILogger<FeedsController>>();
            var controller = new LikesController(context, loggerMock.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserId"] = userId;
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext.HttpContext.User = principal;

            return controller;
        }
    }
}