using FeedApi.Data.Entities;

namespace FeedsApi.DTOs
{
    public class FeedWithLikesDto
    {
        public Feed Feed { get; set; }
        public int Likes { get; set; }
    }
}
