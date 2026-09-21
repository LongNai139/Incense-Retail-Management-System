using SV22T1080045.Shop.BusinessLayers;
using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.Models.ViewModels.Cart;

namespace SV22T1080045.Shop.Models.Mappers
{
    public static class CartMappingExtensions
    {
        public static CartItemViewModel ToViewModel(this CartItem source) => new()
        {
            ProductID = source.ProductID,
            ProductName = source.ProductName,
            Photo = source.Photo,
            Quantity = source.Quantity,
            Price = source.Price
        };

        public static CartUpsellItemViewModel ToUpsellViewModel(this Product source)
        {
            var card = source.ToCardViewModel();
            return new CartUpsellItemViewModel
            {
                ProductID = card.Id,
                ProductName = card.ProductName,
                Photo = card.ImageUrl,
                Price = card.PriceAfterDiscount
            };
        }
    }
}
