using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.Models.Requests.Auth
{
    public class UserRegisterRequest
    {
        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } = "";

        [Required]
        [RegularExpression(@"^(0|\+84)[3-9]\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; } = "";

        [Required]
        [MinLength(8, ErrorMessage = "Mật khẩu tối thiểu 8 ký tự.")]
        public string Password { get; set; } = "";

        [Required]
        public string OtpCode { get; set; } = "";
    }
}
