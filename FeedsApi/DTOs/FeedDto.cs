using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeedsApi.DTOs
{
    public class FeedDto
    {
        public string Title { get; set; }
        public string Description { get; set; }

        [NotMapped]
        [Required]
        public string Discriminator { get; set; }
    }
}
