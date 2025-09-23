using Microsoft.EntityFrameworkCore;

namespace FeedsApi.Data.Entities
{
    [PrimaryKey(nameof(UserId), nameof(FeedId))]
    public class Like
    {
        public int UserId { get; set; }
        public int FeedId { get; set; }
    }
}
