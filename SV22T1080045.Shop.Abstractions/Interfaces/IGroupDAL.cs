using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    /// <summary>
    /// Data Access Layer cho Groups (nhóm quyền)
    /// </summary>
    public interface IGroupDAL
    {
        /// <summary>
        /// Lấy tất cả groups
        /// </summary>
        List<Group> List();

        /// <summary>
        /// Lấy group theo ID
        /// </summary>
        Group? GetById(int id);

        /// <summary>
        /// Lấy groups không phải hệ thống
        /// </summary>
        List<Group> ListNonSystem();

        /// <summary>
        /// Lấy groups với roles của nó
        /// </summary>
        List<Group> ListWithRoles();

        /// <summary>
        /// Thêm group mới
        /// </summary>
        int Add(Group group);

        /// <summary>
        /// Cập nhật group
        /// </summary>
        bool Update(Group group);

        /// <summary>
        /// Xóa group (chỉ group không phải hệ thống)
        /// </summary>
        bool Delete(int id);

        /// <summary>
        /// Kiểm tra group name đã tồn tại chưa
        /// </summary>
        bool IsNameExists(string name, int excludeId = 0);
    }
}
