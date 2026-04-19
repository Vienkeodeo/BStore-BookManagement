using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BookStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        // THÔNG TIN TÀI KHOẢN MOMO SANDBOX (Bạn có thể tự đăng ký hoặc dùng tạm key test này)
        // Key này là public test key của MoMo, có thể dùng để làm đồ án
        private readonly string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
        private readonly string partnerCode = "MOMO5RGX20191128";
        private readonly string accessKey = "M8brj9K6E22vXoDB";
        private readonly string secretKey = "nqQiVSgDMy809JoPF6OzP5OdBUB550Y4";

        [HttpPost("momo")]
        [AllowAnonymous]
        public async Task<IActionResult> CreatePaymentUrl([FromBody] MomoRequestModel request)
        {
            // 1. Chuẩn bị dữ liệu gửi lên MoMo
            string orderInfo = "Thanh toan don hang " + request.OrderId;
            string returnUrl = "http://localhost:5173/my-orders"; // Thanh toán xong quay về trang này
            string notifyUrl = "http://localhost:5173/my-orders"; // (IPN) Thường là API backend để update trạng thái, làm đồ án tạm trỏ về web
            string amount = request.Amount.ToString();
            string orderId = request.OrderId + "_" + DateTime.Now.Ticks.ToString(); // Mã đơn phải là duy nhất
            string requestId = Guid.NewGuid().ToString();
            string extraData = "";

            // 2. Tạo chữ ký bảo mật (Signature)
            string rawHash = $"accessKey={accessKey}&amount={amount}&extraData={extraData}&ipnUrl={notifyUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={returnUrl}&requestId={requestId}&requestType=captureWallet";
            
            string signature = ComputeHmacSha256(rawHash, secretKey);

            // 3. Đóng gói JSON
            var message = new
            {
                partnerCode = partnerCode,
                partnerName = "Test",
                storeId = "MomoTestStore",
                requestId = requestId,
                amount = amount,
                orderId = orderId,
                orderInfo = orderInfo,
                redirectUrl = returnUrl,
                ipnUrl = notifyUrl,
                lang = "vi",
                extraData = extraData,
                requestType = "captureWallet",
                signature = signature
            };

            // 4. Gửi Request sang MoMo
            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<JsonElement>(responseString);

            // 5. Lấy link thanh toán trả về cho Vue.js
            if (responseData.TryGetProperty("payUrl", out JsonElement payUrlElement))
            {
                return Ok(new { payUrl = payUrlElement.GetString() });
            }

            return BadRequest(new { message = "Lỗi kết nối MoMo", details = responseString });
        }

        // Hàm hỗ trợ mã hóa bảo mật
        private string ComputeHmacSha256(string message, string secretKey)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(secretKey);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                string hex = BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
                return hex;
            }
        }
    }

    public class MomoRequestModel
    {
        public string OrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}