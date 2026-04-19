using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreAPI.Data;
using BookStoreAPI.Models;

namespace BookStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Mặc định khóa toàn bộ controller (Chỉ Admin mới vào được)
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =======================================================
        // 1. API CHO ADMIN (POS)
        // =======================================================
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            return await ProcessOrder(request);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = status;
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật trạng thái thành công!" });
        }


        // =======================================================
        // 2. API CHO KHÁCH HÀNG (CLIENT SIDE)
        // =======================================================

        // Khách hàng đặt hàng từ trang Checkout.vue
        [HttpPost("public")]
        [AllowAnonymous] // Mở cửa cho khách không cần Token Admin
        public async Task<IActionResult> CreatePublicOrder([FromBody] CreateOrderRequest request)
        {
            return await ProcessOrder(request);
        }

        // Khách hàng xem lịch sử đơn hàng của mình
        [HttpGet("my-orders/{username}")]
        [AllowAnonymous] // Mở cửa cho khách
        public async Task<ActionResult<IEnumerable<Order>>> GetMyOrders(string username)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book) // Lấy thêm info Sách để hiện tên/ảnh bìa
                .Where(o => o.Username == username) // Chỉ lấy đơn của user này
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(orders);
        }


        // =======================================================
        // 3. HÀM XỬ LÝ CHUNG (DÙNG CHO CẢ ADMIN VÀ KHÁCH)
        // =======================================================
        private async Task<IActionResult> ProcessOrder(CreateOrderRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    CustomerName = string.IsNullOrWhiteSpace(request.CustomerName) ? "Khách lẻ" : request.CustomerName,
                    PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? "0000000000" : request.PhoneNumber,
                    ShippingAddress = string.IsNullOrWhiteSpace(request.ShippingAddress) ? "Mua tại quầy" : request.ShippingAddress,
                    Username = request.Username, // Nhận Username từ FE gửi lên
                    OrderDate = DateTime.Now,
                    TotalAmount = 0,
                    Status = "Pending"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                decimal totalAmount = 0;

                foreach (var item in request.OrderDetails)
                {
                    var book = await _context.Books.FindAsync(item.BookId);

                    if (book == null) return NotFound(new { message = $"Không tìm thấy sách có mã {item.BookId}" });
                    if (book.StockQuantity < item.Quantity) return BadRequest(new { message = $"Sách '{book.Title}' không đủ tồn kho!" });

                    book.StockQuantity -= item.Quantity;
                    _context.Entry(book).State = EntityState.Modified;

                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        BookId = item.BookId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    };

                    _context.OrderDetails.Add(orderDetail);
                    totalAmount += (item.Quantity * item.Price);
                }

                order.TotalAmount = totalAmount;
                _context.Entry(order).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Chốt đơn hàng thành công!", orderId = order.OrderId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Lỗi hệ thống khi tạo đơn hàng: " + ex.Message });
            }
        }
        // Khách hàng tự hủy đơn
        [HttpPut("{id}/cancel")]
        [AllowAnonymous]
        public async Task<IActionResult> CancelOrder(int id, [FromBody] CancelOrderRequest request)
        {
            // 1. Tìm đơn hàng kèm theo chi tiết đơn (để lấy số lượng sách)
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound(new { message = "Không tìm thấy đơn hàng!" });

            // 2. Bảo mật: Kiểm tra xem đơn này có đúng của khách hàng đang đăng nhập không
            if (order.Username != request.Username)
                return BadRequest(new { message = "Bạn không có quyền hủy đơn hàng này!" });

            // 3. Logic: Chỉ cho phép hủy khi đơn còn ở trạng thái Pending (Chờ xử lý)
            if (order.Status != "Pending")
                return BadRequest(new { message = "Chỉ có thể hủy đơn hàng đang chờ xử lý!" });

            // 4. QUAN TRỌNG: Hoàn lại số lượng sách vào kho
            foreach (var detail in order.OrderDetails)
            {
                var book = await _context.Books.FindAsync(detail.BookId);
                if (book != null)
                {
                    book.StockQuantity += detail.Quantity; // Cộng lại số lượng sách
                    _context.Entry(book).State = EntityState.Modified;
                }
            }

            // 5. Đổi trạng thái đơn thành Cancelled
            order.Status = "Cancelled";
            _context.Entry(order).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Hủy đơn hàng thành công!" });
        }
    }

    // =======================================================
    // 4. CÁC CLASS DTO HỨNG DỮ LIỆU
    // =======================================================
    public class CreateOrderRequest
    {
        public string? CustomerName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ShippingAddress { get; set; }
        public string? Username { get; set; } // ĐÃ BỔ SUNG TRƯỜNG NÀY
        public List<OrderItemRequest> OrderDetails { get; set; } = new List<OrderItemRequest>();
    }

    public class OrderItemRequest
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class CancelOrderRequest
    {
        public string Username { get; set; } = string.Empty;
    }
}