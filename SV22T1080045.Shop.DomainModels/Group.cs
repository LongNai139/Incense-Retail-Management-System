using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SV22T1080045.Shop.DomainModels
{
    /// <summary>
    /// Group/Nhóm quyền - để nhóm các Role lại
    /// Giúp quản lý dễ dàng hơn khi số lượng Role lớn
    /// </summary>
    [Table("Groups")]
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Group hệ thống không cho phép xóa
        /// </summary>
        public bool IsSystem { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<RoleGroup> RoleGroups { get; set; } = new List<RoleGroup>();
        public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}
