using Microsoft.Extensions.Caching.Memory;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class UnitService : IUnitService
    {
        private readonly IUnitDAL _unitDal;
        private readonly IMemoryCache _cache;
        private const string UnitsCacheKey = "AllUnits";
        private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(1);

        public UnitService(IUnitDAL unitDal, IMemoryCache cache)
        {
            _unitDal = unitDal;
            _cache = cache;
        }

        public List<Unit> ListUnits(int take = 0)
        {
            string cacheKey = take > 0 ? $"{UnitsCacheKey}_Take{take}" : UnitsCacheKey;
            
            return _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _cacheDuration;
                return _unitDal.ListUnits(take);
            });
        }

        public Unit? GetById(int id)
            => _unitDal.GetById(id);

        public int Add(Unit unit)
        {
            var id = _unitDal.Add(unit);
            InvalidateCache();
            return id;
        }

        public bool Update(Unit unit)
        {
            var result = _unitDal.Update(unit);
            if (result) InvalidateCache();
            return result;
        }

        public bool Delete(int id)
        {
            var result = _unitDal.Delete(id);
            if (result) InvalidateCache();
            return result;
        }

        public void InvalidateCache()
        {
            _cache.Remove(UnitsCacheKey);
            _cache.Remove($"{UnitsCacheKey}_Take10");
            _cache.Remove($"{UnitsCacheKey}_Take20");
            _cache.Remove($"{UnitsCacheKey}_Take50");
        }
    }
}
