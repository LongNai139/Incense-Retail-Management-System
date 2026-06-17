namespace SV22T1080045.Shop.Models.ViewModels.Cart
{
    public class CartPageViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new();
        public List<CartUpsellItemViewModel> UpsellProducts { get; set; } = new();

        public decimal Subtotal => Items.Sum(i => i.TotalPrice);
        public int ItemCount => Items.Sum(i => i.Quantity);
    }

    public class CartUpsellItemViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public string? Photo { get; set; }
        public decimal Price { get; set; }
    }
}
