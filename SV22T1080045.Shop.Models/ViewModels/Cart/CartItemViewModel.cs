using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.Models.ViewModels.Cart
{
    public class CartItemViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public string? Photo { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }
}
