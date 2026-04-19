using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreAPI.Data;

namespace BookStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu đăng nhập
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. API lấy các con số thống kê tổng quan
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            // Tính tổng doanh thu (Bỏ qua các đơn đã Hủy)
            var totalRevenue = await _context.Orders
                                    .Where(o => o.Status != "Cancelled")
                                    .SumAsync(o => o.TotalAmount);

            var totalOrders = await _context.Orders.CountAsync();
            var totalBooks = await _context.Books.SumAsync(b => b.StockQuantity);
            var lowStockCount = await _context.Books.CountAsync(b => b.StockQuantity <= 5);

            return Ok(new
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TotalBooks = totalBooks,
                LowStockCount = lowStockCount
            });
        }

        // 2. API lấy danh sách chi tiết các sách sắp hết hàng
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockBooks()
        {
            var books = await _context.Books
                .Where(b => b.StockQuantity <= 5)
                .Select(b => new { b.BookId, b.Title, b.StockQuantity, b.CoverImageUrl })
                .OrderBy(b => b.StockQuantity)
                .ToListAsync();

            return Ok(books);
        }
    }
}