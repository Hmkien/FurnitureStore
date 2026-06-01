using FurnitureStore.API.Common.Exceptions;

namespace FurnitureStore.API.Middlewares
{
    /// <summary>
    /// Bắt mọi exception chưa xử lý và trả về JSON dạng { error } thống nhất,
    /// thay cho việc lặp try/catch ở từng controller.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                _logger.LogWarning(ex, "Lỗi nghiệp vụ: {Message}", ex.Message);
                await WriteResponse(context, ex.StatusCode, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                await WriteResponse(context, StatusCodes.Status401Unauthorized, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không mong muốn");
                var message = _env.IsDevelopment() ? ex.Message : "Có lỗi xảy ra, vui lòng thử lại";
                await WriteResponse(context, StatusCodes.Status500InternalServerError, message);
            }
        }

        private static Task WriteResponse(HttpContext context, int statusCode, string error)
        {
            if (context.Response.HasStarted)
                return Task.CompletedTask;

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsJsonAsync(new { error });
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
