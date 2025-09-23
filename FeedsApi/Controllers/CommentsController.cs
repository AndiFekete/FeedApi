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

        public CommentsController(FeedDbContext context)
        {
            _context = context;
        }

        [HttpGet("feed/{id}/all-comments")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsForFeed(int id)
        {
            var comments = await _context.Comments.Where(comment => comment.FeedId == id).ToListAsync();

            return Ok(comments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Feed>> PostComment([FromBody] CommentDto commentDto)
        {
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
                return BadRequest("No comment with given id");
            }

            //check if the user is the owner of the comment
            if (actualComment.UserId != HttpContext.GetLoggedInUserId())
            {
                return Forbid();
            }

            //can't update on different feed
            if (actualComment.FeedId != commentDto.FeedId)
            {
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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            if (comment.UserId != HttpContext.GetLoggedInUserId())
            {
                return Forbid();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(comment => comment.Id == id);
        }
    }
}
