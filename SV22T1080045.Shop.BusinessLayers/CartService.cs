//using Microsoft.AspNetCore.Http; 
//using SV22T1080045.Shop.DataLayers;
//using SV22T1080045.Shop.DomainModels;
//using System.Text.Json;

//namespace SV22T1080045.Shop.BusinessLayers
//{
//    public class CartService
//    {
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        private readonly ProductDAL _productDAL; 
//        private const string CART_SESSION_KEY = "SHOP_CART";
//        public CartService(IHttpContextAccessor httpContextAccessor, ProductDAL productDAL)
//        {
//            _httpContextAccessor = httpContextAccessor;
//            _productDAL = productDAL;
//        }

//        // Lấy giỏ hàng hiện tại
//        public List<CartItem> GetCart()
//        {
//            var session = _httpContextAccessor.HttpContext?.Session;
//            string? jsonCart = session?.GetString(CART_SESSION_KEY);

//            if (string.IsNullOrEmpty(jsonCart))
//                return new List<CartItem>();

//            return JsonSerializer.Deserialize<List<CartItem>>(jsonCart) ?? new List<CartItem>();
//        }

//        // Lưu giỏ hàng 
//        private void SaveCart(List<CartItem> cart)
//        {
//            var session = _httpContextAccessor.HttpContext?.Session;
//            string jsonCart = JsonSerializer.Serialize(cart);
//            session?.SetString(CART_SESSION_KEY, jsonCart);
//        }

//        // Thêm vào giỏ
//        public void AddToCart(int productID, int quantity)
//        {
//            var cart = GetCart();
//            var item = cart.FirstOrDefault(x => x.ProductID == productID);

//            if (item == null)
//            {
//                var product = _productDAL.GetProduct(productID);
//                if (product != null)
//                {
//                    cart.Add(new CartItem
//                    {
//                        ProductID = product.ProductID,
//                        ProductName = product.ProductName,
//                        Photo = product.Photo,
//                        Price = product.Price,
//                        Quantity = quantity
//                    });
//                }
//            }
//            else
//            {
//                item.Quantity += quantity;
//            }
//            SaveCart(cart);
//        }

//        // Xóa khỏi giỏ
//        public void RemoveFromCart(int productID)
//        {
//            var cart = GetCart();
//            var item = cart.FirstOrDefault(x => x.ProductID == productID);
//            if (item != null)
//            {
//                cart.Remove(item);
//                SaveCart(cart);
//            }
//        }

//        // Cập nhật số lượng
//        public void UpdateQuantity(int productID, int quantity)
//        {
//            var cart = GetCart();
//            var item = cart.FirstOrDefault(x => x.ProductID == productID);
//            if (item != null && quantity > 0)
//            {
//                item.Quantity = quantity;
//                SaveCart(cart);
//            }
//        }

//        // Xóa sạch giỏ (Sau khi đặt hàng xong)
//        public void ClearCart()
//        {
//            var session = _httpContextAccessor.HttpContext?.Session;
//            session?.Remove(CART_SESSION_KEY);
//        }
//    }
//}