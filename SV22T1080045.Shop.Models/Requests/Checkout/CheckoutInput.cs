using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.Models.Requests.Checkout
{
    public class CheckoutInput
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string ShippingName { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^(0|\+84)[3-9]\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string ShippingPhone { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string ShippingAddress { get; set; } = "";

        public string? Note { get; set; }
        public string? VoucherCode { get; set; }
        public int PaymentMethod { get; set; } = 1; // 1=COD, 2=VNPay, 3=MoMo, 4=Bank QR
    }
}
