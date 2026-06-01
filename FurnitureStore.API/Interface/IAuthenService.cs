using FurnitureStore.API.Models.Entities;
using System.Security.Claims;

namespace FurnitureStore.API.Interface
{
    public interface IAuthenService
    {
        string GenerateJwtToken(User user);

        bool ValidateToken(string token);

        Task<string> GenerateRefreshTokenAsync(Guid userId);

        Task<bool> ValidateRefreshTokenAsync(string refreshToken, Guid userId);

        void SetRefreshTokenCookie(string refreshToken, DateTime expires);

        string? GetRefreshTokenFromCookie();

        void RemoveRefreshTokenCookie();

        ClaimsPrincipal? GetPrincipalFromToken(string token, bool isRefreshToken);

        Task<User?> FindUserByIdAsync(Guid userId);

        Task<bool> Register(User user, string password);

        Task<User?> GetUserByUserNameAsync(string userName);

        bool VerifyPasswordAsync(User user, string password);
    }
}

