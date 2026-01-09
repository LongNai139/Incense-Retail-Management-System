using SV22T1080045.Shop.DataLayers;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers
{
    public class ProductService
    {
        private readonly ProductDAL _productDAL;

        public ProductService(ProductDAL productDAL)
        {
            _productDAL = productDAL;
        }

        public List<Category> GetCategories()
        {
            return _productDAL.GetCategories();
        }

        public List<Product> Search(string search, int categoryID, decimal min, decimal max)
        {
            return _productDAL.ListProducts(search, categoryID, min, max);
        }

        public Product? GetProduct(int id)
        {
            return _productDAL.GetProduct(id);
        }
    }
}