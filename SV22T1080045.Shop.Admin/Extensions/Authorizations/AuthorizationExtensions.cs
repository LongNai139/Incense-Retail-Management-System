using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SV22T1080045.Shop.DomainModels;
using System.Security.Claims;

namespace SV22T1080045.Shop.Admin.Extensions.Authorizations
{
    /// <summary>
    /// Extension methods cho authorization
    /// Dùng để check permission từ ClaimsPrincipal
    /// </summary>
    public static class AuthorizationExtensions
    {
        /// <summary>
        /// Kiểm tra user có permission cụ thể không
        /// </summary>
        public static bool HasPermission(this ClaimsPrincipal user, string permission)
        {
            if (!user.Identity?.IsAuthenticated == true)
                return false;

            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
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

        /// <summary>
        /// Kiểm tra user có ít nhất 1 permission trong danh sách không
        /// </summary>
        public static bool HasAnyPermission(this ClaimsPrincipal user, params string[] permissions)
        {
            if (!user.Identity?.IsAuthenticated == true)
                return false;

            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userRole))
                return false;

            // Admin có tất cả quyền
            if (userRole == CustomerRoles.Admin)
                return true;

            // Kiểm tra permission theo role
            if (CustomerRoles.RolePermissions.TryGetValue(userRole, out var rolePermissions))
            {
                return permissions.Any(p => rolePermissions.Contains(p));
            }

            return false;
        }

        /// <summary>
        /// Kiểm tra user có tất cả permissions trong danh sách không
        /// </summary>
        public static bool HasAllPermissions(this ClaimsPrincipal user, params string[] permissions)
        {
            if (!user.Identity?.IsAuthenticated == true)
                return false;

            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userRole))
                return false;

            // Admin có tất cả quyền
            if (userRole == CustomerRoles.Admin)
                return true;

            // Kiểm tra permission theo role
            if (CustomerRoles.RolePermissions.TryGetValue(userRole, out var rolePermissions))
            {
                return permissions.All(p => rolePermissions.Contains(p));
            }

            return false;
        }

        /// <summary>
        /// Lấy danh sách permissions của user hiện tại
        /// </summary>
        public static string[] GetPermissions(this ClaimsPrincipal user)
        {
            if (!user.Identity?.IsAuthenticated == true)
                return Array.Empty<string>();

            var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userRole))
                return Array.Empty<string>();

            if (CustomerRoles.RolePermissions.TryGetValue(userRole, out var rolePermissions))
            {
                return rolePermissions.ToArray();
            }

            return Array.Empty<string>();
        }
    }
}
