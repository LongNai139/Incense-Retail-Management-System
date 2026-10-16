using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    /// <summary>
    /// Data Access Layer cho RolePermissions (gán permission cho role)
    /// </summary>
    public interface IRolePermissionDAL
    {
        /// <summary>
        /// Lấy tất cả role permissions của một role
        /// </summary>
        List<RolePermission> GetByRole(int roleId);

        /// <summary>
        /// Lấy tất cả role permissions của một permission
        /// </summary>
        List<RolePermission> GetByPermission(int permissionId);

        /// <summary>
        /// Gán permission cho role
        /// </summary>
        bool Grant(int roleId, int permissionId, int grantedBy);

        /// <summary>
        /// Thu hồi permission từ role
        /// </summary>
        bool Revoke(int roleId, int permissionId);

        /// <summary>
        /// Thu hồi tất cả permissions của một role
        /// </summary>
        bool RevokeAll(int roleId);

        bool ReplaceAll(int roleId, IReadOnlyCollection<int> permissionIds, int grantedBy);


        /// <summary>
        /// Kiểm tra role đã có permission chưa
        /// </summary>
        bool HasPermission(int roleId, int permissionId);
    }
}
