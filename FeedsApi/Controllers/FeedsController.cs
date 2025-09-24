using FeedApi.Data.Entities;
using FeedsApi.DTOs;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<IEnumerable<FeedWithLikesDto>>> GetFeeds()
        {
            var allFeeds = await _context.Feeds
                .Select(feed => new FeedWithLikesDto() 
                {
                    Feed = feed,
                    Likes = _context.Likes.Count(like => like.FeedId == feed.FeedId)
                })
                .ToListAsync();

            return Ok(allFeeds);
        }

        [HttpGet("users/{userId}")]
        public async Task<ActionResult<IEnumerable<FeedWithLikesDto>>> GetFeeds(int userId)
        {
            var allFeeds = await _context.Feeds.Where(feed => feed.UserId == userId)
                .Select(feed => new FeedWithLikesDto()
                {
                    Feed = feed,
                    Likes = _context.Likes.Count(like => like.FeedId == feed.FeedId)
                }).ToListAsync();

            return Ok(allFeeds);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FeedWithLikesDto>> GetFeed(int id)
        {
            var feed = await _context.Feeds.FindAsync(id);

            if (feed == null)
            {
                return NotFound();
            }

            return new FeedWithLikesDto()
            { 
                Feed = feed, 
                Likes = _context.Likes.Count(like => like.FeedId == id) 
            };
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Feed>> PostFeed([FromBody] FeedDto feedDto)
        {
            var userId = HttpContext.GetLoggedInUserId();

            Feed feed = feedDto.Discriminator switch
            {
                "Image" => new ImageFeed
                {
                    Title = feedDto.Title,
                    Description = feedDto.Description,
                    ImageData = (feedDto as ImageFeedDto)?.ImageData
                },
                "Video" => new VideoFeed
                {
                    Title = feedDto.Title,
                    Description = feedDto.Description,
                    Url = (feedDto as VideoFeedDto)?.Url
                },
                _ => new Feed
                {
                    Title = feedDto.Title,
                    Description = feedDto.Description
                }
            };

            feed.UserId = userId;
            _context.Feeds.Add(feed);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetFeed", new { id = feed.FeedId }, feed);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFeed(int id, [FromBody] FeedDto feed)
        {
            var actualFeed = _context.Feeds.FirstOrDefault(f => f.FeedId == id);
            //if not found, there's no feed with id
            if (actualFeed == null)
            {
                return BadRequest("No feed with given id");
            }

            //only update to same feed type
            if (actualFeed.Discriminator != feed.Discriminator)
            {
                return BadRequest("Can't update to different type");
            }

            //check if the user is the owner of the feed
            if (actualFeed.UserId != HttpContext.GetLoggedInUserId())
            {
                return Forbid();
            }

            actualFeed.Title = feed.Title;
            actualFeed.Description = feed.Description;
            if (actualFeed is ImageFeed imageFeed && feed is ImageFeedDto imageFeedDto)
            {
                imageFeed.ImageData = imageFeedDto.ImageData;
            }
            else if (actualFeed is VideoFeed videoFeed && feed is VideoFeedDto videoFeedDto)
            {
                videoFeed.Url = videoFeedDto.Url;
            }
            _context.Entry(actualFeed).State = EntityState.Modified;

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

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeed(int id)
        {
            var feed = await _context.Feeds.FindAsync(id);
            if (feed == null)
            {
                return NotFound();
            }

            if (feed.UserId != HttpContext.GetLoggedInUserId()) {
                return Forbid();
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
