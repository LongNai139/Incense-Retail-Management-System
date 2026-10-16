using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SV22T1080045.Shop.DomainModels
{
    /// <summary>
    /// Gán permission riêng cho User (bypass role)
    /// Dùng cho các trường hợp đặc biệt
    /// </summary>
    [Table("UserPermissions")]
    public class UserPermission
    {
        public int UserId { get; set; }

        public int PermissionId { get; set; }

        /// <summary>
        /// true = cấp quyền, false = thu hồi quyền (override role)
        /// </summary>
        public bool IsGranted { get; set; } = true;

        public DateTime GrantedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// User ID người cấp quyền
        /// </summary>
        public int? GrantedBy { get; set; }

        /// <summary>
        /// Quyền tạm thời, sẽ hết hạn sau thời điểm này
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual Customer? User { get; set; }

        [ForeignKey(nameof(PermissionId))]
        public virtual Permission? Permission { get; set; }
    }
}
