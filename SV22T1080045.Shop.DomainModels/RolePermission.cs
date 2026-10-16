using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SV22T1080045.Shop.DomainModels
{
    /// <summary>
    /// Many-to-Many relationship giữa Role và Permission
    /// Xác định Role có những Permission nào
    /// </summary>
    [Table("RolePermissions")]
    public class RolePermission
    {
        public int RoleId { get; set; }

        public int PermissionId { get; set; }

        public DateTime GrantedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// User ID người cấp quyền
        /// </summary>
        public int? GrantedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(RoleId))]
        public virtual Role? Role { get; set; }

        [ForeignKey(nameof(PermissionId))]
        public virtual Permission? Permission { get; set; }
    }
}
