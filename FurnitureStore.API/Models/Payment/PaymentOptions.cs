namespace FurnitureStore.API.Models.Payment
{
    public class VnpayOptions
    {
        public string TmnCode { get; set; } = string.Empty;
        public string HashSecret { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        public string Command { get; set; } = "pay";
        public string CurrCode { get; set; } = "VND";
        public string Version { get; set; } = "2.1.0";
        public string Locale { get; set; } = "vn";
        public string ReturnUrl { get; set; } = string.Empty;
    }

    public class MomoOptions
    {
        public string MomoApiUrl { get; set; } = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
        public string SecretKey { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string PartnerCode { get; set; } = "MOMO";
        public string RequestType { get; set; } = "captureMoMoWallet";
        public string ReturnUrl { get; set; } = string.Empty;
        public string NotifyUrl { get; set; } = string.Empty;
    }
}
