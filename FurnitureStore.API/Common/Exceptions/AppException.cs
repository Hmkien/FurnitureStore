namespace FurnitureStore.API.Common.Exceptions
{
    /// <summary>
    /// Exception nghiệp vụ có gắn HTTP status code, được map sang response bởi
    /// <see cref="Middlewares.ExceptionHandlingMiddleware"/>.
    /// </summary>
    public abstract class AppException : Exception
    {
        public abstract int StatusCode { get; }

        protected AppException(string message) : base(message)
        {
        }
    }

    /// <summary>Không tìm thấy bản ghi (HTTP 404).</summary>
    public class NotFoundException : AppException
    {
        public override int StatusCode => StatusCodes.Status404NotFound;

        public NotFoundException(string message = "Không tìm thấy bản ghi") : base(message)
        {
        }
    }

    /// <summary>Dữ liệu xung đột, ví dụ trùng UserName/Email (HTTP 409).</summary>
    public class ConflictException : AppException
    {
        public override int StatusCode => StatusCodes.Status409Conflict;

        public ConflictException(string message) : base(message)
        {
        }
    }

    /// <summary>Yêu cầu không hợp lệ (HTTP 400).</summary>
    public class BadRequestException : AppException
    {
        public override int StatusCode => StatusCodes.Status400BadRequest;

        public BadRequestException(string message) : base(message)
        {
        }
    }
}
