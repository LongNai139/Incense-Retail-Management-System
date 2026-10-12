using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using SV22T1080045.Shop.DomainModels;
using System.Security.Claims;

namespace SV22T1080045.Shop.Admin.Extensions.Authorizations
{
    /// <summary>
    /// Custom Authorization Attribute dựa trên permission thay vì role
    /// Sử dụng: [Permission("view_order", "add_order")]
    /// </summary>
    public class PermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _permissions;

        public PermissionAttribute(params string[] permissions)
        {
            _permissions = permissions;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Chưa đăng nhập
            if (!user.Identity?.IsAuthenticated == true)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.ChallengeResult();
                return;
            }

            // Kiểm tra permission
            if (!user.HasAnyPermission(_permissions))
            {
                context.Result = new Microsoft.AspNetCore.Mvc.ForbidResult();
                return;
            }
        }
    }
}
