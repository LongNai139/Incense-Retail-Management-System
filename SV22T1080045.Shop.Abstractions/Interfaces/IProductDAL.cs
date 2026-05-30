using SV22T1080045.Shop.Abstractions.Models;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    public interface IProductDAL
    {
        Product? GetById(int id);
        List<Product> List(string? searchValue = null, int take = 0, string sortBy = "newest");
        List<Product> ListByCategory(int categoryId, int excludeProductId, int take = 0);
        ProductListResult Query(ProductQuery query);

        int Add(Product product);
        bool Update(Product product);
        bool Delete(int id);

        bool IsNameExists(string productName, int id);
        bool CategoryExists(int categoryId);
        bool UnitExists(int unitId);
    }
}
