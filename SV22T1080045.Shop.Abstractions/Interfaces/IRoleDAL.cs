using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    /// <summary>
    /// Data Access Layer cho Roles
    /// </summary>
    public interface IRoleDAL
    {
        /// <summary>
        /// Lấy tất cả roles
        /// </summary>
        List<Role> List();

        /// <summary>
        /// Lấy role theo ID
        /// </summary>
        Role? GetById(int id);

        /// <summary>
        /// Lấy role theo Name
        /// </summary>
        Role? GetByName(string name);

        /// <summary>
        /// Lấy roles không phải hệ thống (để quản lý)
        /// </summary>
        List<Role> ListNonSystem();

        /// <summary>
        /// Thêm role mới
        /// </summary>
        int Add(Role role);

        /// <summary>
        /// Cập nhật role
        /// </summary>
        bool Update(Role role);

        /// <summary>
        /// Xóa role (chỉ role không phải hệ thống)
        /// </summary>
        bool Delete(int id);

        /// <summary>
        /// Kiểm tra role name đã tồn tại chưa
        /// </summary>
        bool IsNameExists(string name, int excludeId = 0);
    }
}
