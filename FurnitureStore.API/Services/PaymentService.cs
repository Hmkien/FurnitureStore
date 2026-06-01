using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FurnitureStore.API.Common.Exceptions;
using FurnitureStore.API.Common.Payment;
using FurnitureStore.API.Data;
using FurnitureStore.API.Interface;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Enums;
using FurnitureStore.API.Models.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FurnitureStore.API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly VnpayOptions _vnpay;
        private readonly MomoOptions _momo;
        private readonly IHttpClientFactory _httpClientFactory;

        public PaymentService(
            ApplicationDbContext context,
            IOptions<VnpayOptions> vnpay,
            IOptions<MomoOptions> momo,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _vnpay = vnpay.Value;
            _momo = momo.Value;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<CreatePaymentResultVM> CreateVnpayUrlAsync(string orderCode, string ipAddress)
        {
            var order = await GetPayableOrderAsync(orderCode);
            var amount = (long)decimal.Round(order.TotalAmount) * 100;

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _vnpay.Version);
            vnpay.AddRequestData("vnp_Command", _vnpay.Command);
            vnpay.AddRequestData("vnp_TmnCode", _vnpay.TmnCode);
            vnpay.AddRequestData("vnp_Amount", amount.ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _vnpay.CurrCode);
            vnpay.AddRequestData("vnp_IpAddr", string.IsNullOrWhiteSpace(ipAddress) ? "127.0.0.1" : ipAddress);
            vnpay.AddRequestData("vnp_Locale", _vnpay.Locale);
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {order.OrderCode}");
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _vnpay.ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", order.OrderCode);

            var url = vnpay.CreateRequestUrl(_vnpay.BaseUrl, _vnpay.HashSecret);
            return new CreatePaymentResultVM { PaymentUrl = url };
        }

        public async Task<PaymentResultVM> HandleVnpayReturnAsync(IQueryCollection query)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in query)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    vnpay.AddResponseData(key, value!);
            }

            var orderCode = vnpay.GetResponseData("vnp_TxnRef");
            var responseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var transactionNo = vnpay.GetResponseData("vnp_TransactionNo");
            var secureHash = query["vnp_SecureHash"].ToString();

            if (!vnpay.ValidateSignature(secureHash, _vnpay.HashSecret))
                return Fail(orderCode, responseCode, "Chữ ký không hợp lệ");

            if (responseCode != "00")
                return Fail(orderCode, responseCode, "Thanh toán thất bại hoặc bị hủy");

            await MarkPaidAsync(orderCode);
            return new PaymentResultVM
            {
                Success = true,
                OrderCode = orderCode,
                TransactionId = transactionNo,
                ResponseCode = responseCode,
                Message = "Thanh toán thành công"
            };
        }

        public async Task<CreatePaymentResultVM> CreateMomoUrlAsync(string orderCode)
        {
            var order = await GetPayableOrderAsync(orderCode);
            var amount = ((long)decimal.Round(order.TotalAmount)).ToString();
            var orderInfo = $"Thanh toan don hang {order.OrderCode}";

            var rawData =
                $"partnerCode={_momo.PartnerCode}" +
                $"&accessKey={_momo.AccessKey}" +
                $"&requestId={order.OrderCode}" +
                $"&amount={amount}" +
                $"&orderId={order.OrderCode}" +
                $"&orderInfo={orderInfo}" +
                $"&returnUrl={_momo.ReturnUrl}" +
                $"&notifyUrl={_momo.NotifyUrl}" +
                $"&extraData=";

            var signature = ComputeHmacSha256(rawData, _momo.SecretKey);

            var payload = new
            {
                accessKey = _momo.AccessKey,
                partnerCode = _momo.PartnerCode,
                requestType = _momo.RequestType,
                notifyUrl = _momo.NotifyUrl,
                returnUrl = _momo.ReturnUrl,
                orderId = order.OrderCode,
                amount,
                orderInfo,
                requestId = order.OrderCode,
                extraData = "",
                signature
            };

            var client = _httpClientFactory.CreateClient();
            using var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            using var response = await client.PostAsync(_momo.MomoApiUrl, content);
            var body = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(body);
            if (!doc.RootElement.TryGetProperty("payUrl", out var payUrl) || string.IsNullOrWhiteSpace(payUrl.GetString()))
                throw new BadRequestException("Không tạo được liên kết thanh toán Momo");

            return new CreatePaymentResultVM { PaymentUrl = payUrl.GetString()! };
        }

        public async Task<PaymentResultVM> HandleMomoReturnAsync(IQueryCollection query)
        {
            var orderCode = query["orderId"].ToString();
            var resultCode = query["resultCode"].ToString();
            var transId = query["transId"].ToString();

            if (resultCode != "0")
                return Fail(orderCode, resultCode, "Thanh toán Momo thất bại hoặc bị hủy");

            await MarkPaidAsync(orderCode);
            return new PaymentResultVM
            {
                Success = true,
                OrderCode = orderCode,
                TransactionId = transId,
                ResponseCode = resultCode,
                Message = "Thanh toán thành công"
            };
        }

        private async Task<Order> GetPayableOrderAsync(string orderCode)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode)
                ?? throw new NotFoundException("Không tìm thấy đơn hàng");

            if (order.PaymentStatus == PaymentStatus.Paid)
                throw new BadRequestException("Đơn hàng đã được thanh toán");

            return order;
        }

        private async Task MarkPaidAsync(string? orderCode)
        {
            if (string.IsNullOrWhiteSpace(orderCode))
                return;

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
            if (order == null || order.PaymentStatus == PaymentStatus.Paid)
                return;

            order.PaymentStatus = PaymentStatus.Paid;
            if (order.OrderStatus == OrderStatus.Pending)
                order.OrderStatus = OrderStatus.Confirmed;

            await _context.SaveChangesAsync();
        }

        private static PaymentResultVM Fail(string? orderCode, string? code, string message) => new()
        {
            Success = false,
            OrderCode = orderCode,
            ResponseCode = code,
            Message = message
        };

        private static string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(messageBytes);
            return Convert.ToHexString(hashBytes).ToLower(CultureInfo.InvariantCulture);
        }
    }
}
