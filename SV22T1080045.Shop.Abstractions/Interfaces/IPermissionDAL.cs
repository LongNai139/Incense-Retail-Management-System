using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    /// <summary>
    /// Data Access Layer cho Permissions
    /// </summary>
    public interface IPermissionDAL
    {
        /// <summary>
        /// Lấy tất cả permissions
        /// </summary>
        List<Permission> List();

        /// <summary>
        /// Lấy permission theo ID
        /// </summary>
        Permission? GetById(int id);

        /// <summary>
        /// Lấy permission theo Code
        /// </summary>
        Permission? GetByCode(string code);

        /// <summary>
        /// Lấy permissions theo module
        /// </summary>
        List<Permission> GetByModule(string module);

        /// <summary>
        /// Lấy permissions của một role
        /// </summary>
        List<Permission> GetByRole(int roleId);

        /// <summary>
        /// Lấy permissions của một user (từ role + user-specific)
        /// </summary>
        List<Permission> GetByUser(int userId);

        /// <summary>
        /// Thêm permission mới
        /// </summary>
        int Add(Permission permission);

        /// <summary>
        /// Cập nhật permission
        /// </summary>
        bool Update(Permission permission);

        /// <summary>
        /// Xóa permission
        /// </summary>
        bool Delete(int id);

        /// <summary>
        /// Kiểm tra permission code đã tồn tại chưa
        /// </summary>
        bool IsCodeExists(string code, int excludeId = 0);
    }
}
