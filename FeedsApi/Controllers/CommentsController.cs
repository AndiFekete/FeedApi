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
    public class CommentsController : ControllerBase
    {
        private readonly FeedDbContext _context;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(FeedDbContext context, ILogger<CommentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("feed/{id}/all-comments")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsForFeed(int id)
        {
            var comments = await _context.Comments.Where(comment => comment.FeedId == id).ToListAsync();

            _logger.LogDebug($"GetCommentsForFeed({id})");
            return Ok(comments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                _logger.LogWarning($"GetComment({id}) NOT FOUND");
                return NotFound();
            }

            _logger.LogDebug($"GetComment({id})");
            return comment;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Feed>> PostComment([FromBody] CommentDto commentDto)
        {
            if (!_context.Feeds.Any(feed => feed.FeedId == commentDto.FeedId))
            {
                _logger.LogWarning($"PostComment() Cannot comment on non existent feed {commentDto.FeedId}");
                return BadRequest("Cannot comment on non existent feed");
            }

            var userId = HttpContext.GetLoggedInUserId();

            var comment = new Comment
            {
                FeedId = commentDto.FeedId,
                UserId = userId,
                Text = commentDto.Text,
                Timestamp = DateTime.UtcNow
            };

            comment.UserId = userId;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            _logger.LogDebug($"PostComment() Commented on feed {commentDto.FeedId}");
            return CreatedAtAction("GetComment", new { id = comment.FeedId }, comment);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(int id, [FromBody] CommentDto commentDto)
        {
            var actualComment = _context.Comments.FirstOrDefault(comment => comment.Id == id);
            //if not found, there's no feed with id
            if (actualComment == null)
            {
                _logger.LogWarning($"PutComment({id}) NOT FOUND");
                return BadRequest("No comment with given id");
            }

            //check if the user is the owner of the comment
            if (actualComment.UserId != HttpContext.GetLoggedInUserId())
            {
                _logger.LogWarning($"PutComment({id}) FORBIDDEN");
                return Forbid();
            }

            //can't update on different feed
            if (actualComment.FeedId != commentDto.FeedId)
            {
                _logger.LogWarning($"PutComment({id}) BAD REQUEST - different feed");
                return BadRequest("Can't update on different feed");
            }

            actualComment.Text = commentDto.Text;
            actualComment.Timestamp = DateTime.UtcNow;

            _context.Entry(actualComment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    _logger.LogWarning($"PutComment({id}) NOT FOUND in concurrency check");
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            _logger.LogDebug($"PutComment({id}) UPDATED");
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                _logger.LogWarning($"DeleteComment({id}) NOT FOUND");
                return NotFound();
            }

            if (comment.UserId != HttpContext.GetLoggedInUserId())
            {
                _logger.LogWarning($"DeleteComment({id}) FORBIDDEN");
                return Forbid();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            _logger.LogDebug($"DeleteComment({id}) deleted");
            return NoContent();
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(comment => comment.Id == id);
        }
    }
}
