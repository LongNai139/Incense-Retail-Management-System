using System.ComponentModel.DataAnnotations;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.Models
{
    public class LoginViewModel
    {
        [Required]
        [RegularExpression(@"^(0|\+84)[3-9]\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; } = "";

        [Required]
        [MinLength(5, ErrorMessage = "Mật khẩu tối thiểu 8 ký tự.")]
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

    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [MaxLength(100)]
        public string CustomerName { get; set; } = "";

        public string Phone { get; set; } = "";

        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }
    }

    public static class AccountViewModelExtensions
    {
        public static Customer ToCustomer(this RegisterViewModel model)
        {
            return new Customer
            {
                CustomerName = model.CustomerName,
                Phone = model.Phone,
                Password = model.Password,
                Role = "Customer"
            };
        }

        public static ProfileViewModel ToProfileViewModel(this Customer customer)
        {
            return new ProfileViewModel
            {
                CustomerName = customer.CustomerName,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address
            };
        }
    }
}
