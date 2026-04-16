using Microsoft.AspNetCore.Http;
using SV22T1080045.Shop.DataLayers;
using SV22T1080045.Shop.DomainModels;
using System.Text.Json;

namespace SV22T1080045.Shop.BusinessLayers
{
    // ── CartItem model ────────────────────────────────────────────────────────
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public string Photo { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }

    // ── Interface (để CheckoutController inject được) ──────────────────────
    public interface ICartService
    {
        List<CartItem> GetCart();
        void AddToCart(int productID, int quantity);
        void RemoveFromCart(int productID);
        void UpdateQuantity(int productID, int quantity);
        void ClearCart();
        int Count();
        decimal Total();
    }

    // ── Implementation (giữ nguyên logic gốc của bạn) ─────────────────────
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductDAL _productDAL;
        private const string CART_SESSION_KEY = "SHOP_CART";

        public CartService(IHttpContextAccessor httpContextAccessor, IProductDAL productDAL)
        {
            _httpContextAccessor = httpContextAccessor;
            _productDAL = productDAL;
        }

        public List<CartItem> GetCart()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            string? json = session?.GetString(CART_SESSION_KEY);
            if (string.IsNullOrEmpty(json)) return new List<CartItem>();
            return JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            session?.SetString(CART_SESSION_KEY, JsonSerializer.Serialize(cart));
        }

        public void AddToCart(int productID, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductID == productID);
            if (item == null)
            {
                var product = _productDAL.GetProduct(productID);
                if (product != null)
                {
                    cart.Add(new CartItem
                    {
                        ProductID = product.Id,
                        ProductName = product.ProductName,
                        Photo = product.ImageUrl ?? "",
                        Price = product.PriceAfterDiscount > 0
                                        ? product.PriceAfterDiscount
                                        : product.OriginalPrice,
                        Quantity = quantity
                    });
                }
            }
            else
            {
                item.Quantity += quantity;
            }
            SaveCart(cart);
        }

        public void RemoveFromCart(int productID)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductID == productID);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }
        }

        public void UpdateQuantity(int productID, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductID == productID);
            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
                SaveCart(cart);
            }
        }

        public void ClearCart()
        {
            _httpContextAccessor.HttpContext?.Session.Remove(CART_SESSION_KEY);
        }

        public int Count() => GetCart().Sum(x => x.Quantity);
        public decimal Total() => GetCart().Sum(x => x.TotalPrice);
    }
}