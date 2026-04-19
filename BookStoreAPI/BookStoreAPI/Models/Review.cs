using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BookStoreAPI.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        [StringLength(100)]
        public string? ReviewerName { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } // Đánh giá từ 1 đến 5 sao

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int BookId { get; set; }

        [ForeignKey("BookId")]
        [JsonIgnore]
        public virtual Book? Book { get; set; }
    }
}