using FurnitureStore.API.Data;
using FurnitureStore.API.Interface;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FurnitureStore.API.Services
{
    public class AuthenService : IAuthenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ICookieService _cookieService;
        private readonly ApplicationDbContext _context;
        private static readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();

        public AuthenService(
            IOptions<JwtSettings> jwtSettings,
            ICookieService cookieService
            , ApplicationDbContext context)
        {
            _jwtSettings = jwtSettings.Value;
            _cookieService = cookieService;
            _context = context;

            if (Encoding.UTF8.GetBytes(_jwtSettings.Key).Length < 32)
            {
                throw new InvalidOperationException("JWT key must be at least 32 bytes long for HMAC-SHA256.");
            }
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(_jwtSettings.ExpireMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                return true;
            }
            catch (SecurityTokenException)
            {
                return false;
            }
        }

        public async Task<User?> FindUserByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public Task<string> GenerateRefreshTokenAsync(Guid userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim("token_type", "refresh")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(7);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var refreshToken = new JwtSecurityTokenHandler().WriteToken(token);
            return Task.FromResult(refreshToken);
        }

        public ClaimsPrincipal? GetPrincipalFromToken(string token, bool isRefreshToken = false)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = !isRefreshToken,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = _tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                if (isRefreshToken && validatedToken is JwtSecurityToken jwtToken)
                {
                    var tokenType = jwtToken.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value;
                    if (tokenType != "refresh")
                        return null;
                }

                return principal;
            }
            catch (SecurityTokenException)
            {
                return null;
            }
        }

        public Task<bool> ValidateRefreshTokenAsync(string refreshToken, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(refreshToken)) return Task.FromResult(false);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = _tokenHandler.ValidateToken(refreshToken, validationParameters, out var validatedToken);

                var tokenUserId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var tokenType = principal.FindFirst("token_type")?.Value;

                if (tokenType != "refresh" || !Guid.TryParse(tokenUserId, out var guid) || guid != userId)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            }
            catch (SecurityTokenException)
            {
                return Task.FromResult(false);
            }
        }

        public void SetRefreshTokenCookie(string refreshToken, DateTime expires)
        {
            _cookieService.SetRefreshTokenCookie(refreshToken, expires);
        }

        public string? GetRefreshTokenFromCookie()
        {
            return _cookieService.GetRefreshTokenFromCookie();
        }

        public void RemoveRefreshTokenCookie()
        {
            _cookieService.RemoveRefreshTokenCookie();
        }

        public async Task<bool> Register(User user, string password)
        {
            var existingUser = await _context.Users
                .Where(u => u.UserName == user.UserName || u.Email == user.Email)
                .Select(u => new { u.UserName, u.Email })
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                if (existingUser.UserName == user.UserName)
                {
                    throw new InvalidOperationException("Tên người dùng đã tồn tại");
                }

                throw new InvalidOperationException("Email đã tồn tại");
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            if (user.UserName == "spadmin")
            {
                user.IsSuperUser = true;
            }
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            return await _context.Users
                .AsNoTracking()
                .Where(u => u.UserName == userName)
                .Select(u => new User
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    PasswordHash = u.PasswordHash,
                    Status = u.Status,
                })
                .FirstOrDefaultAsync();
        }

        public bool VerifyPasswordAsync(User user, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }


    }
}

