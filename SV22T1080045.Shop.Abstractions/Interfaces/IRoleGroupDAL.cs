using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    /// <summary>
    /// Data Access Layer cho RoleGroups (gán role vào group)
    /// </summary>
    public interface IRoleGroupDAL
    {
        /// <summary>
        /// Lấy tất cả role groups của một group
        /// </summary>
        List<RoleGroup> GetByGroup(int groupId);

        /// <summary>
        /// Lấy tất cả role groups của một role
        /// </summary>
        List<RoleGroup> GetByRole(int roleId);

        /// <summary>
        /// Gán role vào group
        /// </summary>
        bool Assign(int groupId, int roleId);

        /// <summary>
        /// Gán nhiều roles vào group
        /// </summary>
        bool AssignMany(int groupId, List<int> roleIds);

        /// <summary>
        /// Xóa role khỏi group
        /// </summary>
        bool Remove(int groupId, int roleId);

        /// <summary>
        /// Xóa tất cả roles của một group
        /// </summary>
        bool RemoveAll(int groupId);

        /// <summary>
        /// Kiểm tra group đã có role chưa
        /// </summary>
        bool HasRole(int groupId, int roleId);
    }
}
