using SV22T1080045.Shop.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.DataLayers
{
    public interface IProductDAL
    {
        List<Product> ListProducts(string searchValue = "", int CategoryId = 0, decimal OriginalPrice = 0, decimal PriceAfterDiscount = 0);
        int AddProduct(Product data);
        bool UpdateProduct(Product data);
        bool DeleteProduct(int id);
        bool InUsed(int id);
        Product GetProduct(int id);
        bool IsNameExists(string productName, int id);
    }
}
