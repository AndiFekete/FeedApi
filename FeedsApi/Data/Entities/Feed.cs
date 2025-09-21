namespace FeedApi.Data.Entities
{
    public class Feed
    {
        public int FeedId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
