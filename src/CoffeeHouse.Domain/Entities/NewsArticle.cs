using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Domain.Entities
{
    public class NewsArticle
    {
        [Key]
        public int ArticleId { get; set; }

        [StringLength(255)]
        public string Title { get; set; } = null!;

        public DateTime PublishedAt { get; set; }

        [Column(TypeName = "TEXT")]
        public string Content { get; set; } = null!;

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Draft";
    }
}
