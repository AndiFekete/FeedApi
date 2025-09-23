using FeedApi.Data.Entities;
using FeedsApi.Data.Entities;
using FeedsApi.DTOs;
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

        public LikesController(FeedDbContext context)
        {
            _context = context;
        }

        [HttpGet("get-liking-users/{feedId}")]
        public async Task<ActionResult<int[]>> GetLikingUsers(int feedId)
        {
            var users = await _context.Likes.Where(like => like.FeedId == feedId).Select(like => like.UserId).ToListAsync();
            return Ok(users);
        }

        [Authorize]
        [HttpPost("like/{feedId}")]
        public async Task<IActionResult> LikeFeed(int feedId)
        {
            var like = new Like() { FeedId = feedId, UserId = HttpContext.GetLoggedInUserId() };
            _context.Likes.Add(like);
            await _context.SaveChangesAsync();
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
                return NotFound();
            }

            if (like.UserId != userId)
            {
                return Forbid();
            }

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
