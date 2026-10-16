using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    /// <summary>
    /// Data Access Layer cho UserPermissions (gán permission riêng cho user)
    /// </summary>
    public interface IUserPermissionDAL
    {
        /// <summary>
        /// Lấy tất cả user permissions của một user
        /// </summary>
        List<UserPermission> GetByUser(int userId);

        /// <summary>
        /// Lấy tất cả user permissions của một permission
        /// </summary>
        List<UserPermission> GetByPermission(int permissionId);

        /// <summary>
        /// Lấy user permissions đang hiệu lực (chưa expire)
        /// </summary>
        List<UserPermission> GetActiveByUser(int userId);

        /// <summary>
        /// Gán permission cho user
        /// </summary>
        bool Grant(int userId, int permissionId, int grantedBy, DateTime? expiresAt = null);

        /// <summary>
        /// Thu hồi permission từ user
        /// </summary>
        bool Revoke(int userId, int permissionId);

        bool ReplaceOverrides(int userId, IReadOnlyCollection<UserPermission> overrides, int grantedBy);

        bool SetOverride(int userId, int permissionId, bool? isGranted, int grantedBy, DateTime? expiresAt = null);

        /// <summary>
        /// Thu hồi tất cả permissions của một user
        /// </summary>
        bool RevokeAll(int userId);

        /// <summary>
        /// Kiểm tra user đã có permission chưa
        /// </summary>
        bool HasPermission(int userId, int permissionId);

        /// <summary>
        /// Xóa các permissions đã expire
        /// </summary>
        int CleanExpired();
    }
}
