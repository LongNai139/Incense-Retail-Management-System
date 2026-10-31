using Microsoft.Extensions.Caching.Memory;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryDAL _categoryDal;
        private readonly IMemoryCache _cache;
        private const string CategoriesCacheKey = "AllCategories";
        private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(1);

        public CategoryService(ICategoryDAL categoryDal, IMemoryCache cache)
        {
            _categoryDal = categoryDal;
            _cache = cache;
        }

        public List<Category> ListCategories(int take = 0)
        {
            string cacheKey = take > 0 ? $"{CategoriesCacheKey}_Take{take}" : CategoriesCacheKey;
            
            return _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
                return _categoryDal.ListCategories(take);
            });
        }

        public Category? GetById(int id)
            => _categoryDal.GetById(id);

        public int Add(Category category)
        {
            var id = _categoryDal.Add(category);
            InvalidateCache();
            return id;
        }

        public bool Update(Category category)
        {
            var result = _categoryDal.Update(category);
            if (result) InvalidateCache();
            return result;
        }

        public bool Delete(int id)
        {
            var result = _categoryDal.Delete(id);
            if (result) InvalidateCache();
            return result;
        }

        public void InvalidateCache()
        {
            _cache.Remove(CategoriesCacheKey);
            _cache.Remove($"{CategoriesCacheKey}_Take10");
            _cache.Remove($"{CategoriesCacheKey}_Take20");
            _cache.Remove($"{CategoriesCacheKey}_Take50");
        }
    }
}
