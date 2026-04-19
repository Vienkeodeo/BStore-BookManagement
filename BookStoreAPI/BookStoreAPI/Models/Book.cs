using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreAPI.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        [StringLength(255)]
        public string? Title { get; set; }

        // Thêm trường này để lưu link ảnh hoặc đường dẫn file ảnh
        public string? CoverImageUrl { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        // Khóa ngoại Thể loại
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        // Khóa ngoại Tác giả
        public int AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public virtual Author? Author { get; set; }

        // Khóa ngoại Nhà xuất bản
        public int PublisherId { get; set; }
        [ForeignKey("PublisherId")]
        public virtual Publisher? Publisher { get; set; }

        // 1 Cuốn sách có nhiều đánh giá
        public virtual ICollection<Review>? Reviews { get; set; }
    }
}