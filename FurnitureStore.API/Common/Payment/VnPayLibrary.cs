using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace FurnitureStore.API.Common.Payment
{
    /// <summary>
    /// Tiện ích VNPAY: tích lũy tham số, tạo URL thanh toán có chữ ký và
    /// xác thực chữ ký phản hồi (HMACSHA512 trên query string đã sort Ordinal).
    /// </summary>
    public class VnPayLibrary
    {
        private readonly SortedList<string, string> _requestData = new(new VnPayCompare());
        private readonly SortedList<string, string> _responseData = new(new VnPayCompare());

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                _requestData[key] = value;
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                _responseData[key] = value;
        }

        public string? GetResponseData(string key)
            => _responseData.TryGetValue(key, out var value) ? value : null;

        public string CreateRequestUrl(string baseUrl, string hashSecret)
        {
            var data = new StringBuilder();
            foreach (var (key, value) in _requestData)
                data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");

            var queryString = data.ToString();
            if (queryString.Length > 0)
                queryString = queryString.Remove(queryString.Length - 1, 1);

            var signData = queryString;
            var secureHash = HmacSha512(hashSecret, signData);
            return $"{baseUrl}?{queryString}&vnp_SecureHash={secureHash}";
        }

        public bool ValidateSignature(string inputHash, string hashSecret)
        {
            var rawData = GetResponseRaw();
            var computed = HmacSha512(hashSecret, rawData);
            return computed.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string GetResponseRaw()
        {
            _responseData.Remove("vnp_SecureHashType");
            _responseData.Remove("vnp_SecureHash");

            var data = new StringBuilder();
            foreach (var (key, value) in _responseData)
            {
                if (!string.IsNullOrEmpty(value))
                    data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
            }
            if (data.Length > 0)
                data.Remove(data.Length - 1, 1);
            return data.ToString();
        }

        private static string HmacSha512(string key, string inputData)
        {
            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            using var hmac = new HMACSHA512(keyBytes);
            var hashValue = hmac.ComputeHash(inputBytes);
            foreach (var b in hashValue)
                hash.Append(b.ToString("x2"));
            return hash.ToString();
        }
    }

    /// <summary>So sánh key theo Ordinal (en-US) như chuẩn VNPAY yêu cầu.</summary>
    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return CompareInfo.GetCompareInfo("en-US").Compare(x, y, CompareOptions.Ordinal);
        }
    }
}
