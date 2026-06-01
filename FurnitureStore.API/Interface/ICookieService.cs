namespace FurnitureStore.API.Interface
{
    public interface ICookieService
    {
        void SetRefreshTokenCookie(string refreshToken, DateTime expires);

        string? GetRefreshTokenFromCookie();

        void RemoveRefreshTokenCookie();
    }
}

