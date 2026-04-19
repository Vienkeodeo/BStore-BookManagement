using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookStoreAPI.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        public string? Description { get; set; }

        // JsonIgnore giúp tránh lỗi vòng lặp JSON khi API trả về dữ liệu
        [JsonIgnore]
        public virtual ICollection<Book>? Books { get; set; }
    }
}