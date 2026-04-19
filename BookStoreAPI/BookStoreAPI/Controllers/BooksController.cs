using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreAPI.Data;
using BookStoreAPI.Models;

namespace BookStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // BẮT BUỘC ĐĂNG NHẬP (CÓ TOKEN JWT) MỚI ĐƯỢC GỌI API NÀY

    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Books (Lấy danh sách)
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            // Lấy sách kèm luôn cả thông tin Thể loại, Tác giả để hiển thị cho đẹp
            return await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .ToListAsync();
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            // Thêm .Include để lấy thông tin Tác giả, Thể loại, NXB
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null)
            {
                return NotFound(new { message = "Không tìm thấy cuốn sách này!" });
            }

            return book;
        }

        // GET: api/Orders/by-phone/{phone}
        [HttpGet("by-phone/{phone}")]
        [AllowAnonymous] // Cho phép khách vãng lai tra cứu
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByPhone(string phone)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book) // Lấy thêm thông tin sách để hiện ảnh/tên
                .Where(o => o.PhoneNumber == phone)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            if (orders == null || !orders.Any()) return NotFound(new { message = "Không tìm thấy đơn hàng nào cho số điện thoại này!" });

            return orders;
        }
        // POST: api/Books (Thêm sách mới)
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetBook", new { id = book.BookId }, book);
        }

        // PUT: api/Books/5 (Cập nhật sách)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.BookId) return BadRequest(new { message = "ID không khớp" });

            _context.Entry(book).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Books/5 (Xóa sách)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Xóa thành công!" });
        }
    }
}