using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SV22T1080045.Shop.DomainModels;
using System.Security.Claims;

namespace SV22T1080045.Shop.Admin.Extensions.Authorizations
{
    /// <summary>
    /// Extension methods cho authorization
    /// Đọc permission từ claims (được ghi lúc đăng nhập từ database)
    /// </summary>
    public static class AuthorizationExtensions
    {
        /// <summary>
        /// Loại claim chứa danh sách mã quyền, các mã cách nhau bằng dấu phẩy.
        /// Gộp thành một claim để cookie không bị phình to.
        /// </summary>
        public const string PermissionClaimType = "permissions";

        private const char PermissionSeparator = ',';

        /// <summary>
        /// Kiểm tra user có permission cụ thể không
        /// </summary>
        public static bool HasPermission(this ClaimsPrincipal user, string permission)
        {
            if (!IsAuthenticated(user) || string.IsNullOrWhiteSpace(permission))
                return false;

            // Admin có tất cả quyền
            if (IsAdmin(user))
                return true;

            return GetPermissions(user).Contains(permission.Trim(), StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Kiểm tra user có ít nhất 1 permission trong danh sách không
        /// </summary>
        public static bool HasAnyPermission(this ClaimsPrincipal user, params string[] permissions)
        {
            if (!IsAuthenticated(user))
                return false;

            // Admin có tất cả quyền
            if (IsAdmin(user))
                return true;

            var owned = GetPermissions(user);
            return permissions.Any(p => !string.IsNullOrWhiteSpace(p)
                && owned.Contains(p.Trim(), StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Kiểm tra user có tất cả permissions trong danh sách không
        /// </summary>
        public static bool HasAllPermissions(this ClaimsPrincipal user, params string[] permissions)
        {
            if (!IsAuthenticated(user))
                return false;

            // Admin có tất cả quyền
            if (IsAdmin(user))
                return true;

            var owned = GetPermissions(user);
            return permissions.All(p => !string.IsNullOrWhiteSpace(p)
                && owned.Contains(p.Trim(), StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Lấy danh sách permissions của user hiện tại (đọc từ claims)
        /// </summary>
        public static string[] GetPermissions(this ClaimsPrincipal user)
        {
            if (!IsAuthenticated(user))
                return Array.Empty<string>();

            return user.FindAll(PermissionClaimType)
                .SelectMany(c => c.Value.Split(PermissionSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        /// <summary>
        /// Tạo claim permissions từ danh sách mã quyền (dùng khi đăng nhập).
        /// </summary>
        public static Claim CreatePermissionClaim(IEnumerable<string> permissionCodes)
        {
            var value = string.Join(PermissionSeparator, permissionCodes.OrderBy(c => c, StringComparer.Ordinal));
            return new Claim(PermissionClaimType, value);
        }

        private static bool IsAuthenticated(ClaimsPrincipal? user)
            => user?.Identity?.IsAuthenticated == true;

        // Không dùng user.IsInRole để tránh đệ quy khi user là CustomClaimsPrincipal
        private static bool IsAdmin(ClaimsPrincipal user)
            => user.FindAll(ClaimTypes.Role)
                .Any(c => string.Equals(c.Value, CustomerRoles.Admin, StringComparison.OrdinalIgnoreCase));
    }
}