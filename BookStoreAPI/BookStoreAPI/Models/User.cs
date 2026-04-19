using System.ComponentModel.DataAnnotations;

namespace BookStoreAPI.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Thêm trường này: "Admin" hoặc "Customer"
        public string Role { get; set; } = "Customer";
    }
}
