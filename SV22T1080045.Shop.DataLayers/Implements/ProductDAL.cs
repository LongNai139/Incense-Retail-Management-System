using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.Abstractions.Models;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class ProductDAL : IProductDAL
    {
        private readonly ShopDbContext _context;

        public ProductDAL(ShopDbContext context)
        {
            _context = context;
        }

        public ProductListResult Query(ProductQuery query)
        {
            var baseQuery = CreateBaseQuery();
            var filteredQuery = ApplyFilters(baseQuery, query);

            var totalCount = filteredQuery.Count();

            var originOptions = ApplyFilters(baseQuery, query, includeOrigin: false)
                .Where(p => !string.IsNullOrWhiteSpace(p.Origin))
                .Select(p => p.Origin!)
                .Distinct()
                .OrderBy(origin => origin)
                .ToList();

            var originCounts = ApplyFilters(baseQuery, query, includeOrigin: false)
                .Where(p => !string.IsNullOrWhiteSpace(p.Origin))
                .GroupBy(p => p.Origin!)
                .Select(group => new { group.Key, Count = group.Count() })
                .ToDictionary(x => x.Key, x => x.Count);

            var categoryCounts = ApplyFilters(baseQuery, query, includeCategory: false)
                .GroupBy(p => p.CategoryId)
                .Select(group => new { group.Key, Count = group.Count() })
                .ToDictionary(x => x.Key, x => x.Count);

            var products = ApplySorting(filteredQuery, query.SortBy)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return new ProductListResult
            {
                Products = products,
                TotalCount = totalCount,
                OriginOptions = originOptions,
                OriginCounts = originCounts,
                CategoryCounts = categoryCounts
            };
        }

        public Product? GetById(int id)
        {
            return CreateBaseQuery()
                .FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        }

        public List<Product> List(string? searchValue = null, int take = 0, string sortBy = "newest")
        {
            var query = CreateBaseQuery();

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                var keyword = searchValue.Trim();
                query = query.Where(p => p.ProductName.Contains(keyword));
            }

            query = ApplySorting(query, sortBy);

            if (take > 0)
                query = query.Take(take);

            return query.ToList();
        }

        public List<Product> ListByCategory(int categoryId, int excludeProductId, int take = 0)
        {
            IQueryable<Product> query = CreateBaseQuery()
                .Where(p => p.CategoryId == categoryId && p.Id != excludeProductId)
                .OrderByDescending(p => p.CreatedTime);

            if (take > 0)
                query = query.Take(take);

            return query.ToList();
        }

        public int Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return product.Id;
        }

        public bool Update(Product product)
        {
            var existing = _context.Products
                .Include(p => p.Inventory)
                .FirstOrDefault(p => p.Id == product.Id && !p.IsDeleted);

            if (existing == null)
                return false;

            _context.Entry(existing).CurrentValues.SetValues(product);
            if (product.Inventory != null)
            {
                product.Inventory.ProductId = product.Id;
                if (existing.Inventory == null)
                {
                    existing.Inventory = product.Inventory;
                }
                else
                {
                    _context.Entry(existing.Inventory).CurrentValues.SetValues(product.Inventory);
                }
            }

            return _context.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (product == null)
                return false;

            product.IsDeleted = true;
            return _context.SaveChanges() > 0;
        }

        public bool IsNameExists(string productName, int id)
        {
            return _context.Products
                .Any(p => !p.IsDeleted && p.ProductName == productName && p.Id != id);
        }

        public bool CategoryExists(int categoryId)
        {
            return _context.Categories.Any(c => c.Id == categoryId && !c.IsDeleted);
        }

        public bool UnitExists(int unitId)
        {
            return _context.Units.Any(u => u.Id == unitId && !u.IsDeleted);
        }

        private IQueryable<Product> CreateBaseQuery()
        {
            return _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Unit)
                .Include(p => p.Inventory)
                .Where(p => !p.IsDeleted);
        }

        private static IQueryable<Product> ApplyFilters(
            IQueryable<Product> query,
            ProductQuery filter,
            bool includeCategory = true,
            bool includeOrigin = true)
        {
            if (!string.IsNullOrWhiteSpace(filter.SearchValue))
                query = query.Where(p => p.ProductName.Contains(filter.SearchValue.Trim()));

            if (includeCategory && filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

            if (filter.PriceMin.HasValue)
                query = query.Where(p =>
                    (p.PriceAfterDiscount > 0
                        ? p.PriceAfterDiscount
                        : (p.SalePrice > 0 ? p.SalePrice * (100 - p.DiscountPercent) / 100 : p.OriginalPrice)) >= filter.PriceMin.Value);

            if (filter.PriceMax.HasValue)
                query = query.Where(p =>
                    (p.PriceAfterDiscount > 0
                        ? p.PriceAfterDiscount
                        : (p.SalePrice > 0 ? p.SalePrice * (100 - p.DiscountPercent) / 100 : p.OriginalPrice)) <= filter.PriceMax.Value);

            if (includeOrigin && !string.IsNullOrWhiteSpace(filter.Origin))
                query = query.Where(p => p.Origin == filter.Origin);

            if (!string.IsNullOrWhiteSpace(filter.UsageTag))
            {
                var tag = $",{filter.UsageTag.Trim()},";
                query = query.Where(p =>
                    p.UsageTags != null &&
                    ("," + p.UsageTags + ",").Contains(tag));
            }

            if (filter.MinRating.HasValue)
                query = query.Where(p => (p.Rating ?? 0) >= filter.MinRating.Value);

            return query;
        }

        private static IQueryable<Product> ApplySorting(IQueryable<Product> query, string? sortBy)
        {
            return sortBy switch
            {
                "bestseller" => query.OrderByDescending(p => p.SoldCount ?? 0).ThenByDescending(p => p.CreatedTime),
                "newest" => query.OrderByDescending(p => p.CreatedTime),
                "price-asc" => query.OrderBy(p => p.PriceAfterDiscount > 0 ? p.PriceAfterDiscount : (p.SalePrice > 0 ? p.SalePrice * (100 - p.DiscountPercent) / 100 : p.OriginalPrice)),
                "price-desc" => query.OrderByDescending(p => p.PriceAfterDiscount > 0 ? p.PriceAfterDiscount : (p.SalePrice > 0 ? p.SalePrice * (100 - p.DiscountPercent) / 100 : p.OriginalPrice)),
                "rating" => query.OrderByDescending(p => p.Rating ?? 0).ThenByDescending(p => p.CreatedTime),
                _ => query.OrderByDescending(p => p.CreatedTime)
            };
        }
    }
}
