using Microsoft.AspNetCore.Http;
using SV22T1080045.Shop.BusinessLayers.Helpers;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using System.Text.Json;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class CartService : ICartService
    {
        private const string CartSessionKey = "SHOP_CART";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductService _productService;

        public CartService(IHttpContextAccessor httpContextAccessor, IProductService productService)
        {
            _httpContextAccessor = httpContextAccessor;
            _productService = productService;
        }

        public List<CartItem> GetCart()
        {
            var json = _httpContextAccessor.HttpContext?.Session.GetString(CartSessionKey);
            if (string.IsNullOrWhiteSpace(json))
                return new List<CartItem>();

            return JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
        }

        public void AddToCart(int productID, int quantity)
        {
            if (quantity <= 0)
                return;

            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductID == productID);
            if (item != null)
            {
                item.Quantity += quantity;
                SaveCart(cart);
                return;
            }

            var product = _productService.GetProduct(productID);
            if (product == null)
                return;

            cart.Add(new CartItem
            {
                ProductID = product.Id,
                ProductName = product.ProductName,
                Photo = ProductImageUrlHelper.Normalize(product.ImageUrl) ?? "",
                Price = product.DisplayPrice,
                Quantity = quantity
            });

            SaveCart(cart);
        }

        public void RemoveFromCart(int productID)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductID == productID);
            if (item == null)
                return;

            cart.Remove(item);
            SaveCart(cart);
        }

        public void UpdateQuantity(int productID, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductID == productID);
            if (item == null)
                return;

            if (quantity <= 0)
            {
                cart.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }

            SaveCart(cart);
        }

        public void ClearCart()
        {
            _httpContextAccessor.HttpContext?.Session.Remove(CartSessionKey);
        }

        public int Count() => GetCart().Sum(x => x.Quantity);

        public decimal Total() => GetCart().Sum(x => x.TotalPrice);

        private void SaveCart(List<CartItem> cart)
        {
            _httpContextAccessor.HttpContext?.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }
    }
}
