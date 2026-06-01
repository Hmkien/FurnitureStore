using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace FurnitureStore.API.Extensions
{
    public static class ExtensionConvert
    {
        #region Enum Extensions

        /// <summary>
        /// Lấy Description attribute của enum value
        /// </summary>
        public static string GetDescription(this Enum value)
        {
            if (value == null)
                return string.Empty;

            FieldInfo? fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null)
                return value.ToString();

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        /// Lấy enum value từ Description
        /// </summary>
        public static T? GetEnumFromDescription<T>(this string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T?)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T?)field.GetValue(null);
                }
            }
            return default;
        }

        #endregion

        #region String Extensions - Vietnamese

        /// <summary>
        /// Chuyển chuỗi tiếng Việt có dấu thành không dấu
        /// </summary>
        public static string RemoveVietnameseTones(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            string[] vietnameseSigns = new string[]
            {
                "aAeEoOuUiIdDyY",
                "áàạảãâấầậẩẫăắằặẳẵ",
                "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                "éèẹẻẽêếềệểễ",
                "ÉÈẸẺẼÊẾỀỆỂỄ",
                "óòọỏõôốồộổỗơớờợởỡ",
                "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                "úùụủũưứừựửữ",
                "ÚÙỤỦŨƯỨỪỰỬỮ",
                "íìịỉĩ",
                "ÍÌỊỈĨ",
                "đ",
                "Đ",
                "ýỳỵỷỹ",
                "ÝỲỴỶỸ"
            };

            for (int i = 1; i < vietnameseSigns.Length; i++)
            {
                for (int j = 0; j < vietnameseSigns[i].Length; j++)
                {
                    text = text.Replace(vietnameseSigns[i][j], vietnameseSigns[0][i - 1]);
                }
            }

            return text;
        }

        /// <summary>
        /// Tạo slug từ chuỗi (dùng cho URL)
        /// </summary>
        public static string ToSlug(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Chuyển thành chữ thường
            text = text.ToLowerInvariant();

            // Bỏ dấu tiếng Việt
            text = text.RemoveVietnameseTones();

            // Thay thế ký tự đặc biệt và khoảng trắng bằng dấu gạch ngang
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
            text = Regex.Replace(text, @"\s+", " ").Trim();
            text = Regex.Replace(text, @"\s", "-");
            text = Regex.Replace(text, @"-+", "-");

            return text;
        }

        /// <summary>
        /// Tạo slug kèm số tự động (dành cho trùng lặp)
        /// </summary>
        public static string ToSlugWithNumber(this string text, int number)
        {
            var slug = text.ToSlug();
            return number > 1 ? $"{slug}-{number}" : slug;
        }

        #endregion

        #region String Extensions - General

        /// <summary>
        /// Viết hoa chữ cái đầu
        /// </summary>
        public static string ToTitleCase(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
        }

        /// <summary>
        /// Cắt chuỗi và thêm dấu ...
        /// </summary>
        public static string Truncate(this string text, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength).TrimEnd() + suffix;
        }

        /// <summary>
        /// Kiểm tra chuỗi có phải email hợp lệ
        /// </summary>
        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra số điện thoại Việt Nam hợp lệ
        /// </summary>
        public static bool IsValidVietnamesePhoneNumber(this string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            var regex = new Regex(@"^(0|\+84)(3[2-9]|5[6|8|9]|7[0|6-9]|8[1-9]|9[0-9])[0-9]{7}$");
            return regex.IsMatch(phoneNumber);
        }

        /// <summary>
        /// Loại bỏ ký tự đặc biệt
        /// </summary>
        public static string RemoveSpecialCharacters(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            return Regex.Replace(text, @"[^a-zA-Z0-9\s]", "");
        }

        /// <summary>
        /// Chuyển chuỗi thành Base64
        /// </summary>
        public static string ToBase64(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            byte[] bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Giải mã Base64 thành chuỗi
        /// </summary>
        public static string FromBase64(this string base64Text)
        {
            if (string.IsNullOrWhiteSpace(base64Text))
                return string.Empty;

            byte[] bytes = Convert.FromBase64String(base64Text);
            return Encoding.UTF8.GetString(bytes);
        }

        #endregion

        #region DateTime Extensions

        /// <summary>
        /// Chuyển DateTime sang định dạng dd/MM/yyyy
        /// </summary>
        public static string ToVietnameseDateString(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Chuyển DateTime sang định dạng dd/MM/yyyy HH:mm:ss
        /// </summary>
        public static string ToVietnameseDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
        }

        /// <summary>
        /// Tính số ngày so với hôm nay
        /// </summary>
        public static int DaysFromNow(this DateTime dateTime)
        {
            return (dateTime.Date - DateTime.Now.Date).Days;
        }

        /// <summary>
        /// Lấy ngày đầu tháng
        /// </summary>
        public static DateTime StartOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        /// <summary>
        /// Lấy ngày cuối tháng
        /// </summary>
        public static DateTime EndOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
        }

        /// <summary>
        /// Kiểm tra có phải ngày trong tương lai
        /// </summary>
        public static bool IsFuture(this DateTime dateTime)
        {
            return dateTime > DateTime.Now;
        }

        /// <summary>
        /// Kiểm tra có phải ngày trong quá khứ
        /// </summary>
        public static bool IsPast(this DateTime dateTime)
        {
            return dateTime < DateTime.Now;
        }

        #endregion

        #region Number Extensions

        /// <summary>
        /// Định dạng số tiền theo format Việt Nam
        /// </summary>
        public static string ToVietnameseCurrency(this decimal amount)
        {
            return amount.ToString("#,##0", CultureInfo.GetCultureInfo("vi-VN")) + " đ";
        }

        /// <summary>
        /// Định dạng số tiền theo format Việt Nam (int)
        /// </summary>
        public static string ToVietnameseCurrency(this int amount)
        {
            return ((decimal)amount).ToVietnameseCurrency();
        }

        /// <summary>
        /// Định dạng phần trăm
        /// </summary>
        public static string ToPercentage(this decimal value, int decimals = 2)
        {
            return $"{Math.Round(value * 100, decimals)}%";
        }

        /// <summary>
        /// Làm tròn đến hàng nghìn
        /// </summary>
        public static decimal RoundToThousand(this decimal value)
        {
            return Math.Round(value / 1000) * 1000;
        }

        #endregion

        #region Collection Extensions

        /// <summary>
        /// Kiểm tra collection có rỗng hay null
        /// </summary>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? collection)
        {
            return collection == null || !collection.Any();
        }

        /// <summary>
        /// Chia list thành các batch nhỏ
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            var batch = new List<T>(batchSize);
            foreach (var item in source)
            {
                batch.Add(item);
                if (batch.Count == batchSize)
                {
                    yield return batch;
                    batch = new List<T>(batchSize);
                }
            }
            if (batch.Count > 0)
                yield return batch;
        }

        /// <summary>
        /// Chuyển IEnumerable thành chuỗi với separator
        /// </summary>
        public static string JoinToString<T>(this IEnumerable<T> source, string separator = ", ")
        {
            return string.Join(separator, source);
        }

        #endregion

        #region Object Extensions

        /// <summary>
        /// Kiểm tra object có null hay không
        /// </summary>
        public static bool IsNull(this object? obj)
        {
            return obj == null;
        }

        /// <summary>
        /// Kiểm tra object không null
        /// </summary>
        public static bool IsNotNull(this object? obj)
        {
            return obj != null;
        }

        #endregion

        #region Password Extensions

        /// <summary>
        /// Tạo mật khẩu ngẫu nhiên
        /// </summary>
        public static string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
            var random = new Random();
            var chars = new char[length];

            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(chars);
        }

        /// <summary>
        /// Tạo mã code ngẫu nhiên (chỉ chữ và số)
        /// </summary>
        public static string GenerateRandomCode(int length = 8)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var chars = new char[length];

            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(chars);
        }

        #endregion
    }
}

