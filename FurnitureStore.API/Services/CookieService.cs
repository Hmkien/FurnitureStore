using FurnitureStore.API.Interface;

namespace FurnitureStore.API.Services
{
    public class CookieService : ICookieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetRefreshTokenCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            };
            _httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        public void RemoveRefreshTokenCookie()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                Console.WriteLine("HttpContext is null in RemoveRefreshTokenCookie");
                return;
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(-1)
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Delete("refreshToken", cookieOptions);
        }

        public string? GetRefreshTokenFromCookie()
        {
            return _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
        }
    }
}

