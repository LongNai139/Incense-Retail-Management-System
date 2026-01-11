using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.Models;
using System.Runtime.CompilerServices;

namespace SV22T1080045.Shop.Admin.AppCodes.Mappers
{
    public static class ProductMapping
    {
        public static ProductEditModel ToEditModel(this Product target)
        {
            return new ProductEditModel()
            {
                Id = target.Id,
                ProductName = target.ProductName,
                UnitId = target.UnitId,
                CategoryId = target.CategoryId,
                OriginalPrice = target.OriginalPrice,
                PriceAfterDiscount = target.PriceAfterDiscount,
                BurningTime = target.BurningTime,
                Ingredient = target.Ingredient,
                Description = target.Description,
                ImageUrl = target.ImageUrl
            };
        }
        public static Product ToDomainModel(this ProductEditModel target)
        {
            return new Product()
            {
                Id = target.Id,
                ProductName = target.ProductName,
                UnitId = target.UnitId,
                CategoryId = target.CategoryId,
                OriginalPrice = target.OriginalPrice,
                PriceAfterDiscount = target.PriceAfterDiscount,
                BurningTime = target.BurningTime,
                Ingredient = target.Ingredient,
                Description = target.Description,
                ImageUrl = target.ImageUrl
            };
        }
    }
}
