using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookStoreAPI.Models
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }

        [Required]
        [StringLength(100)]
        public string? FullName { get; set; }

        public string? Biography { get; set; }

        [JsonIgnore]
        public virtual ICollection<Book>? Books { get; set; }
    }
}