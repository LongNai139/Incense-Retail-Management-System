using SV22T1080045.Shop.Models.Requests.Checkout;
using SV22T1080045.Shop.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.Models.ViewModels.Checkout
{
    public class CheckoutViewModel
    {
        // ── Danh sách sản phẩm trong giỏ ─────────────────────────────────
        public List<CartItemViewModel> CartItems { get; set; } = new();

        public decimal TotalAmount => CartItems.Sum(i => i.TotalPrice);
        public decimal FinalAmount => TotalAmount - DiscountAmount;
        public decimal DiscountAmount { get; set; } = 0;

        // ── Dữ liệu form (prefill hoặc re-fill khi validation fail) ───────
        public CheckoutInput Input { get; set; } = new();
    }

    
}
