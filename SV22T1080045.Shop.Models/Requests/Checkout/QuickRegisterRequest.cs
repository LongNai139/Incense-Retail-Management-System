using System.ComponentModel.DataAnnotations;
using SV22T1080045.Shop.Abstractions;

namespace SV22T1080045.Shop.Models.Requests.Checkout
{
    public class QuickRegisterRequest
    {
        [Required]
        [RegularExpression(PhoneNumberHelper.VietnameseMobilePattern, ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; } = "";

        [Required]
        [MinLength(8, ErrorMessage = "Mật khẩu tối thiểu 8 ký tự.")]
        public string Password { get; set; } = "";

        public string? CustomerName { get; set; }

        [Range(1, int.MaxValue)]
        public int OrderId { get; set; }
    }
}
