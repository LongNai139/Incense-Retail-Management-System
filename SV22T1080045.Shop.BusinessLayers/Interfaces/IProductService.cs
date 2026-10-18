using SV22T1080045.Shop.Abstractions.Contracts.Products;
using SV22T1080045.Shop.Abstractions.Models;
using SV22T1080045.Shop.Abstractions.Models.Management;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Interfaces
{
    public interface IProductService
    {
        ProductListResult ListProductsFiltered(ProductQuery query);
        ProductDashboardData GetManagementDashboardData();
        int GetNewestPageForProduct(int productId, int pageSize);
        List<Product> ListProducts(string? searchValue = null, int take = 0, string sortBy = "newest");
        List<Product> ListRelatedProducts(int productId, int categoryId, int take = 0);
        Product? GetProduct(int id);
        int SaveProduct(ProductSaveRequest request);
        bool DeleteProduct(int id);
        bool UpdateInventoryQuantity(int productId, int quantityChange);
    }
}
