using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SV22T1080045.Shop.DomainModels
{
    /// <summary>
    /// Role/Vai trò người dùng trong hệ thống
    /// Thay thế hard-code string 'Admin', 'Staff', 'Customer'
    /// </summary>
    [Table("Roles")]
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = "";  // 'Admin', 'Staff', 'Customer'

        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; } = "";

        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Role hệ thống không cho phép xóa
        /// </summary>
        public bool IsSystem { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public virtual ICollection<RoleGroup> RoleGroups { get; set; } = new List<RoleGroup>();
        public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}
