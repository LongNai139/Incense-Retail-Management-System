using Microsoft.AspNetCore.Http;
using SV22T1080045.Shop.DomainModels;
using System.Security.Claims;

namespace SV22T1080045.Shop.Admin.Extensions.Authorizations
{
    /// <summary>
    /// Custom ClaimsPrincipal để kiểm tra permission từ database
    /// Thay vì chỉ check role cứng, nó check permission granular
    /// </summary>
    public class CustomClaimsPrincipal : ClaimsPrincipal
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomClaimsPrincipal(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor?.HttpContext?.User)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Override IsInRole để kiểm tra permission thay vì role
        /// </summary>
        public override bool IsInRole(string role)
        {
            if (!Identity?.IsAuthenticated == true)
                return false;

            // Nếu role là role gốc (Admin, Staff, Customer), check role bình thường
            if (role == CustomerRoles.Admin || role == CustomerRoles.Staff || role == CustomerRoles.Customer)
            {
                return base.IsInRole(role);
            }

            // Nếu role là permission, kiểm tra user có permission đó không
            return HasPermission(role);
        }

        /// <summary>
        /// Kiểm tra user có ít nhất 1 permission trong danh sách không
        /// </summary>
        public bool IsInPermissions(params string[] permissions)
        {
            if (!Identity?.IsAuthenticated == true)
                return false;

            var userRole = (Identity as ClaimsPrincipal)?.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userRole))
                return false;

            // Lấy danh sách permission của role
            if (CustomerRoles.RolePermissions.TryGetValue(userRole, out var rolePermissions))
            {
                return permissions.Any(p => rolePermissions.Contains(p));
            }

            return false;
        }

        /// <summary>
        /// Kiểm tra user có permission cụ thể không
        /// </summary>
        private bool HasPermission(string permission)
        {
            var userRole = (Identity as ClaimsPrincipal)?.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userRole))
                return false;

            // Admin có tất cả quyền
            if (userRole == CustomerRoles.Admin)
                return true;

            // Kiểm tra permission theo role
            if (CustomerRoles.RolePermissions.TryGetValue(userRole, out var rolePermissions))
            {
                return rolePermissions.Contains(permission);
            }

            return false;
        }
    }
}
