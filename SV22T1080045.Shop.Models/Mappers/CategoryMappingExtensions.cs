using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.Models.ViewModels.Home;
using SV22T1080045.Shop.Models.ViewModels.Product;

namespace SV22T1080045.Shop.Models.Mappers
{
    public static class CategoryMappingExtensions
    {
        public static CategoryViewModel ToCategoryViewModel(this Category source)
        {
            return new CategoryViewModel
            {
                Id = source.Id,
                CategoryName = source.CategoryName
            };
        }

        public static CategoryFilterItem ToCategoryFilterItem(this Category source, int productCount)
        {
            return new CategoryFilterItem
            {
                Id = source.Id,
                CategoryName = source.CategoryName,
                ProductCount = productCount
            };
        }
    }
}
