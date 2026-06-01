using FurnitureStore.API.Interface;
using System.Security.Claims;

namespace FurnitureStore.API.Middlewares
{
    /// <summary>
    /// Middleware để load thông tin user vào RequestContext
    /// </summary>
    public class UserContextMiddleware
    {
        private readonly RequestDelegate _next;

        public UserContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, RequestContext requestContext, IUserService userService)
        {
            var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
            {
                try
                {
                    // Load thông tin user từ database
                    var currentUser = await userService.GetUserDtoByIdAsync(userId);

                    if (currentUser != null)
                    {
                        // Lấy token từ header
                        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader) &&
                            authHeader.FirstOrDefault()?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            currentUser.Token = authHeader.First()!.Substring("Bearer ".Length).Trim();
                        }

                        // Set vào RequestContext
                        requestContext.CurrentUser = currentUser;
                    }
                }
                catch (Exception)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Người dùng không hợp lệ!");
                    return;
                }
            }

            await _next(context);
        }
    }
    public static class UserContextMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserContext(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserContextMiddleware>();
        }
    }
}

