using SV22T1080045.Shop.DataLayers;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers
{
    public interface IProductService
    {
        List<Product> ListProducts(string searchValue = "", int CategoryId = 0, decimal OriginalPrice = 0, decimal PriceAfterDiscount = 0);
        int AddProduct(Product data);
        bool UpdateProduct(Product data);
        bool DeleteProduct(int id);
        bool InUsed(int id);
        Product? GetProduct(int id);
    }
    public class ProductService : IProductService
    {
        private readonly IProductDAL _productDAL;

        public ProductService(IProductDAL productDAL)
        {
            _productDAL = productDAL;
        }
        public List<Product> ListProducts(string searchValue = "", int CategoryId = 0, decimal OriginalPrice = 0, decimal PriceAfterDiscount = 0)
        {
            return _productDAL.ListProducts(searchValue, CategoryId, OriginalPrice, PriceAfterDiscount);
        }
        public int AddProduct(Product data)
        {
            if (string.IsNullOrEmpty(data.ProductName)) return 0;
            if(_productDAL.IsNameExists(data.ProductName, data.Id)) return -1;
            return _productDAL.AddProduct(data);
        }
        public bool UpdateProduct(Product data)
        {
            return _productDAL.UpdateProduct(data);
        }
        public bool DeleteProduct(int id)
        {
            return _productDAL.DeleteProduct(id);
        }
        public Product? GetProduct(int id)
        {
            return _productDAL.GetProduct(id);
        }
        public bool InUsed(int id)
        {
            return _productDAL.InUsed(id);
        }
    }
}
