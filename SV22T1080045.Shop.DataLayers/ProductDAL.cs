using Dapper;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers
{
    public class ProductDAL : IProductDAL
    {
        private readonly ShopDbContext _context;
        public ProductDAL(ShopDbContext context)
        {
            _context = context;
        }

        public List<Product> ListProducts(string searchValue = "", int CategoryId = 0, decimal OriginalPrice = 0, decimal PriceAfterDiscount = 0)
        {
            return _context.Products.ToList();
        }

        public Product GetProduct(int id)
        {
            return _context.Products.Find(id);
        }

        public int AddProduct(Product data)
        {
            try
            {
                _context.Products.Add(data);
                _context.SaveChanges();
                return data.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateProduct(Product data)
        {
            try
            {
                _context.Products.Update(data);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteProduct(int id)
        {
            try
            {
                var data = _context.Products.Find(id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool InUsed(int id)
        {
            throw new NotImplementedException();
        }

        public bool IsNameExists(string productName, int id)
        {
            return _context.Products.Any(p => p.ProductName == productName && p.Id != id);
        }
    }
}