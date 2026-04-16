using SV22T1080045.Shop.DomainModels;
using System.Collections.Generic;
using System.Linq;

namespace SV22T1080045.Shop.DataLayers
{
    // ── INTERFACE ─────────────────────────────
    public interface IProductDAL
    {
        List<Product> ListProducts(string searchValue = "", int CategoryId = 0, decimal OriginalPrice = 0, decimal PriceAfterDiscount = 0);
        int AddProduct(Product data);
        bool UpdateProduct(Product data);
        bool DeleteProduct(int id);
        bool InUsed(int id);
        Product? GetProduct(int id);
        bool IsNameExists(string productName, int id);
    }

    // ── IMPLEMENT ─────────────────────────────
    public class ProductDAL : IProductDAL
    {
        private readonly ShopDbContext _context;

        public ProductDAL(ShopDbContext context)
        {
            _context = context;
        }

        public List<Product> ListProducts(string searchValue = "", int CategoryId = 0, decimal OriginalPrice = 0, decimal PriceAfterDiscount = 0)
        {
            var query = _context.Products.Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                searchValue = searchValue.Trim();
                query = query.Where(p => p.ProductName.Contains(searchValue));
            }

            if (CategoryId > 0)
                query = query.Where(p => p.CategoryId == CategoryId);

            if (OriginalPrice > 0)
                query = query.Where(p => p.OriginalPrice >= OriginalPrice);

            if (PriceAfterDiscount > 0)
                query = query.Where(p => (p.PriceAfterDiscount > 0 ? p.PriceAfterDiscount : p.OriginalPrice) <= PriceAfterDiscount);

            return query
                .OrderByDescending(p => p.CreatedTime)
                .ToList();
        }

        public Product? GetProduct(int id)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        }

        public int AddProduct(Product data)
        {
            _context.Products.Add(data);
            _context.SaveChanges();
            return data.Id;
        }

        public bool UpdateProduct(Product data)
        {
            _context.Products.Update(data);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteProduct(int id)
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

        public bool InUsed(int id)
        {
            throw new System.NotImplementedException();
        }

        public bool IsNameExists(string productName, int id)
        {
            return _context.Products.Any(p => p.ProductName == productName && p.Id != id);
        }
    }
}