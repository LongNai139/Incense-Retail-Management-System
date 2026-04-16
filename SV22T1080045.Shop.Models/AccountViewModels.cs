using System.ComponentModel.DataAnnotations;

namespace SV22T1080045.Shop.Models
{
    public class LoginViewModel
    {
        [Required]
        [RegularExpression(@"^(0|\+84)[3-9]\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; } = "";

        [Required]
        [MinLength(8, ErrorMessage = "Mật khẩu tối thiểu 8 ký tự.")]
        public string Password { get; set; } = "";
    }

    public class RegisterViewModel
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

    public class SendOtpViewModel
    {
        [Required]
        [RegularExpression(@"^(0|\+84)[3-9]\d{8}$")]
        public string Phone { get; set; } = "";
    }

    public class VerifyOtpViewModel
    {
        [Required]
        [RegularExpression(@"^(0|\+84)[3-9]\d{8}$")]
        public string Phone { get; set; } = "";

        [Required]
        [RegularExpression(@"^\d{6}$")]
        public string Code { get; set; } = "";
    }
}
