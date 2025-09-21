using FeedApi.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FeedsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedsController : ControllerBase
    {
        private readonly FeedDbContext _context;

        public FeedsController(FeedDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Feed>>> GetFeeds()
        {
            var allFeeds = await _context.Feeds.ToListAsync();

            return Ok(allFeeds);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Feed>> GetFeed(int id)
        {
            var feed = await _context.Feeds.FindAsync(id);

            if (feed == null)
            {
                return NotFound();
            }

            return feed;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutFeed(int id, Feed feed)
        {
            if (id != feed.FeedId)
            {
                return BadRequest();
            }

            _context.Entry(feed).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedExists(id))
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

        [HttpPost]
        public async Task<ActionResult<Feed>> PostFeed([FromBody] Feed feed)
        {
            _context.Feeds.Add(feed);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetFeed", new { id = feed.FeedId }, feed);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeed(int id)
        {
            var feed = await _context.Feeds.FindAsync(id);
            if (feed == null)
            {
                return NotFound();
            }

            _context.Feeds.Remove(feed);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FeedExists(int id)
        {
            return _context.Feeds.Any(e => e.FeedId == id);
        }
    }
}
