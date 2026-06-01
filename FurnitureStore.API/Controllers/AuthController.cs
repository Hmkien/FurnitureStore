using FurnitureStore.API.Interface;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Enums;
using FurnitureStore.API.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurnitureStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenService _authenService;
        private readonly IConfiguration _configuration;
        private const int REFRESH_TOKEN_VALIDITY_DAYS = 7;

        public AuthController(
            IAuthenService authenService,
            IConfiguration configuration)
        {
            _authenService = authenService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestVM request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Thông tin đăng nhập không hợp lệ");
            }

            try
            {
                var user = await _authenService.GetUserByUserNameAsync(request.Username);
                if (user == null || !_authenService.VerifyPasswordAsync(user, request.Password))
                {
                    await Task.Delay(100);
                    return Unauthorized("Tên đăng nhập hoặc mật khẩu không đúng");
                }
                if (user.Status != StatusEntity.Approved)
                {
                    return Unauthorized("Tài khoản đã bị khóa");
                }

                return await GenerateAuthResponse(user);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Có lỗi xảy ra, vui lòng thử lại" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestVM request)
        {
            try
            {
                var user = new User
                {
                    UserName = request.UserName,
                    Email = request.Email,
                };

                await _authenService.Register(user, request.Password);
                return Ok(new { message = "Đăng ký thành công" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { error = "Đã xảy ra lỗi trong quá trình đăng ký" });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestVM request)
        {
            var refreshToken = _authenService.GetRefreshTokenFromCookie();
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                refreshToken = request?.RefreshToken;
            }
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized(new { error = "Refresh token không hợp lệ" });
            }

            var principal = _authenService.GetPrincipalFromToken(refreshToken, isRefreshToken: true);
            if (principal == null)
            {
                return Unauthorized(new { error = "Refresh token không hợp lệ" });
            }
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized(new { error = "Refresh token không hợp lệ" });
            }

            var user = await _authenService.FindUserByIdAsync(userId);
            if (user == null || !await _authenService.ValidateRefreshTokenAsync(refreshToken, userId))
            {
                return Unauthorized(new { error = "Refresh token không hợp lệ" });
            }

            return await GenerateAuthResponse(user);
        }

        [HttpPost("check-token")]
        public async Task<IActionResult> CheckToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var guid))
            {
                return Unauthorized("Token không hợp lệ");
            }

            var user = await _authenService.FindUserByIdAsync(guid);
            if (user == null)
            {
                return Unauthorized("Không tìm thấy thông tin người dùng");
            }

            return Ok(new
            {
                id = user.Id,
                userName = user.UserName,
                email = user.Email
            });
        }

        [Authorize]
        [HttpDelete("logout")]
        public IActionResult Logout()
        {
            _authenService.RemoveRefreshTokenCookie();
            return Ok(new { message = "Đăng xuất thành công" });
        }

        private async Task<IActionResult> GenerateAuthResponse(User user)
        {
            var accessToken = _authenService.GenerateJwtToken(user);
            var refreshToken = await _authenService.GenerateRefreshTokenAsync(user.Id);
            var refreshTokenExpires = DateTime.Now.AddDays(REFRESH_TOKEN_VALIDITY_DAYS);

            _authenService.SetRefreshTokenCookie(refreshToken, refreshTokenExpires);
            var accessTokenExpiresMinutes = Convert.ToDouble(_configuration["Jwt:ExpireMinutes"] ?? "15");
            var response = new AuthResponseVM
            {
                Username = user.UserName,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpires = DateTime.Now.AddMinutes(accessTokenExpiresMinutes),
                RefreshTokenExpires = refreshTokenExpires
            };

            return Ok(response);
        }
    }
}

