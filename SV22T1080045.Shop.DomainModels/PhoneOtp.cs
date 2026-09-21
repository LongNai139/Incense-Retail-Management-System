using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.DomainModels
{
    /// <summary>
    /// Lưu OTP tạm thời cho xác thực SĐT.
    /// Thực tế nên dùng Redis (TTL tự xóa) — bảng này dùng cho môi trường dev không có Redis.
    /// </summary>
    public class PhoneOtp : _BaseEntity
    {
        public string Phone { get; set; } = "";

        /// <summary>Mã OTP 6 số</summary>
        public string OtpCode { get; set; } = "";

        /// <summary>Mục đích: "login" | "register" | "order_lookup"</summary>
        public string Purpose { get; set; } = "";

        /// <summary>Thời điểm hết hạn (mặc định 5 phút)</summary>
        public DateTime ExpiresAt { get; set; } = DateTime.Now.AddMinutes(5);

        /// <summary>Đã dùng chưa — dùng xong phải đánh dấu để tránh tái sử dụng</summary>
        public bool IsUsed { get; set; } = false;

        /// <summary>Số lần nhập sai</summary>
        public int FailCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsValid() => !IsUsed && ExpiresAt > DateTime.Now && FailCount < 5;
    }
}
