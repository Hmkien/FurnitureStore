using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FurnitureStore.API.Middlewares
{
    /// <summary>
    /// Attribute để kiểm tra quyền truy cập (Authorization)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _permissions;
        private readonly bool _requireAll;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="permissions">Danh sách quyền cần kiểm tra</param>
        /// <param name="requireAll">true: cần tất cả quyền, false: chỉ cần 1 quyền</param>
        public RequirePermissionAttribute(params string[] permissions)
        {
            _permissions = permissions;
            _requireAll = false;
        }

        public RequirePermissionAttribute(bool requireAll, params string[] permissions)
        {
            _permissions = permissions;
            _requireAll = requireAll;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var requestContext = context.HttpContext.RequestServices.GetService<RequestContext>();

            if (requestContext == null || !requestContext.IsAuthenticated)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    error = "Unauthorized",
                    message = "Bạn cần đăng nhập để truy cập tài nguyên này"
                });
                return;
            }

            // SuperUser có tất cả quyền
            if (requestContext.IsSuperUser())
            {
                return;
            }

            // Kiểm tra quyền
            bool hasPermission = _requireAll
                ? requestContext.HasAllPermissions(_permissions)
                : requestContext.HasAnyPermission(_permissions);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }

    /// <summary>
    /// Attribute để kiểm tra role
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequireRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public RequireRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var requestContext = context.HttpContext.RequestServices.GetService<RequestContext>();

            if (requestContext == null || !requestContext.IsAuthenticated)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    error = "Unauthorized",
                    message = "Bạn cần đăng nhập để truy cập tài nguyên này"
                });
                return;
            }

            // SuperUser có tất cả role
            if (requestContext.IsSuperUser())
            {
                return;
            }

            // Kiểm tra role
            bool hasRole = _roles.Any(role => requestContext.HasRole(role));

            if (!hasRole)
            {
                context.Result = new ForbidResult();
            }
        }
    }

    /// <summary>
    /// Attribute yêu cầu SuperUser
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireSuperUserAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var requestContext = context.HttpContext.RequestServices.GetService<RequestContext>();

            if (requestContext == null || !requestContext.IsAuthenticated)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    error = "Unauthorized",
                    message = "Bạn cần đăng nhập để truy cập tài nguyên này"
                });
                return;
            }

            if (!requestContext.IsSuperUser())
            {
                context.Result = new ForbidResult();
            }
        }
    }
}

