using BookStoreAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // HÀM ĐĂNG KÝ (Dùng chung cho cả Admin nếu muốn, mặc định cấp quyền Customer)
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto request)
        {
            // 1. Kiểm tra tài khoản tồn tại chưa
            var userExists = await _userManager.FindByNameAsync(request.Username);
            if (userExists != null)
                return BadRequest(new { message = "Tên tài khoản đã tồn tại!" });

            // 2. Tạo User mới theo chuẩn Identity
            IdentityUser user = new()
            {
                Email = request.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = request.Username
            };

            // 3. Lưu vào database (Tự động mã hóa mật khẩu)
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                // Trả về lỗi chi tiết nếu mật khẩu quá yếu (thường yêu cầu chữ hoa, số, ký tự đặc biệt)
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { message = $"Đăng ký thất bại: {errors}" });
            }

            // 4. Gắn mác "Customer" cho tài khoản này bằng Claim
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Customer"));

            return Ok(new { message = "Tạo tài khoản thành công!" });
        }

        // HÀM ĐĂNG NHẬP (Giữ nguyên logic cũ của bạn)
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username!);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password!))
            {
                // Lấy các phân quyền (Claims) của user từ Database
                var userClaims = await _userManager.GetClaimsAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                // Gộp chung các Claims (để Token biết ai là Admin, ai là Customer)
                authClaims.AddRange(userClaims);

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    username = user.UserName // Trả thêm username về cho Vue.js hiển thị
                });
            }
            return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu!" });
        }
    }

    // Các Class Models để nhận dữ liệu từ Frontend
    public class UserRegisterDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class LoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}