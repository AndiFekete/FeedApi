using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedsApi.Data.Entities
{
    public class Feed
    {
        public int FeedId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [NotMapped]
        [Required]
        public string Discriminator { get; set; }
    }
}
