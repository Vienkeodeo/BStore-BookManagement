using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookStoreAPI.Models
{
    public class Publisher
    {
        [Key]
        public int PublisherId { get; set; }

        [Required]
        [StringLength(150)]
        public string? Name { get; set; }

        public string? Address { get; set; }

        public string? Website { get; set; }

        [JsonIgnore]
        public virtual ICollection<Book>? Books { get; set; }
    }
}