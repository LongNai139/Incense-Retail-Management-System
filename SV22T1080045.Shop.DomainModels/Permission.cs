using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SV22T1080045.Shop.DomainModels
{
    /// <summary>
    /// Permission/Quyền nghiệp vụ cụ thể
    /// Ví dụ: view_order, add_order, delete_order
    /// </summary>
    [Table("Permissions")]
    public class Permission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Code { get; set; } = "";  // 'view_order', 'add_order', ...

        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; } = "";

        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Module nghiệp vụ: Orders, Products, Customers, etc.
        /// </summary>
        [MaxLength(50)]
        public string? Module { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
}
