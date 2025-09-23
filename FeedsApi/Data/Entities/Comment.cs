namespace FeedsApi.Data.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FeedId { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
