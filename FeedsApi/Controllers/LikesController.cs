using FeedsApi.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FeedsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly FeedDbContext _context;
        private readonly ILogger<FeedsController> _logger;

        public LikesController(FeedDbContext context, ILogger<FeedsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("get-liking-users/{feedId}")]
        public async Task<ActionResult<int[]>> GetLikingUsers(int feedId)
        {
            var users = await _context.Likes.Where(like => like.FeedId == feedId).Select(like => like.UserId).ToListAsync();
            
            _logger.LogDebug($"GetLikingUsers({feedId}) retrieved {users.Count} users");
            return Ok(users);
        }

        [Authorize]
        [HttpPost("like/{feedId}")]
        public async Task<IActionResult> LikeFeed(int feedId)
        {
            if (!_context.Feeds.Any(feed => feed.FeedId == feedId))
            {
                _logger.LogWarning($"LikeFeed({feedId}) BAD REQUEST - Cannot like non-existent feed");
                return BadRequest("Cannot like non-existent feed");
            }

            var like = new Like() { FeedId = feedId, UserId = HttpContext.GetLoggedInUserId() };
            _context.Likes.Add(like);
            await _context.SaveChangesAsync();
            _logger.LogDebug($"LikeFeed({feedId}) User {like.UserId} liked feed");
            return Created();
        }

        [Authorize]
        [HttpDelete("unlike/{feedId}")]
        public async Task<IActionResult> UnlikeFeed(int feedId)
        {
            var userId = HttpContext.GetLoggedInUserId();
            var like = await _context.Likes.FindAsync(userId, feedId);
            if (like == null)
            {
                _logger.LogWarning($"UnlikeFeed({feedId}) NOT FOUND");
                return NotFound();
            }

            if (like.UserId != userId)
            {
                _logger.LogWarning($"UnlikeFeed({feedId}) FORBIDDEN - User {userId} trying to unlike for user {like.UserId}");
                return Forbid();
            }

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            _logger.LogDebug($"UnlikeFeed({feedId}) User {userId} unliked feed");
            return NoContent();
        }
    }
}
