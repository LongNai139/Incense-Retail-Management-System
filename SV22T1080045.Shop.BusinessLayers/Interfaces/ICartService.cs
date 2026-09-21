using SV22T1080045.Shop.BusinessLayers;

namespace SV22T1080045.Shop.BusinessLayers.Interfaces
{
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
}
