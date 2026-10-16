using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.BusinessLayers.Interfaces
{
    /// <summary>
    /// Kiểm tra quyền của người dùng (permission lấy từ Role + quyền riêng của user), có cache.
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>
        /// Lấy tập mã quyền (Permission.Code) của user. Kết quả được cache.
        /// </summary>
        IReadOnlySet<string> GetUserPermissionCodes(int userId);

        /// <summary>
        /// Kiểm tra user có quyền theo mã quyền hay không.
        /// </summary>
        bool HasPermission(int userId, string permissionCode);

        /// <summary>
        /// Xóa cache quyền của một user (gọi khi đổi role/group hoặc cấp/thu hồi quyền riêng của user đó).
        /// </summary>
        void InvalidateUser(int userId);

        /// <summary>
        /// Xóa cache quyền của toàn bộ user (gọi khi thay đổi quyền của một Role hoặc Group).
        /// </summary>
        void InvalidateAll();
    }
}