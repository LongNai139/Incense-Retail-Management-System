using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SV22T1080045.Shop.DomainModels
{
    /// <summary>
    /// Many-to-Many relationship giữa Group và Role
    /// Xác định Group chứa những Role nào
    /// </summary>
    [Table("RoleGroups")]
    public class RoleGroup
    {
        public int GroupId { get; set; }

        public int RoleId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(GroupId))]
        public virtual Group? Group { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual Role? Role { get; set; }
    }
}
