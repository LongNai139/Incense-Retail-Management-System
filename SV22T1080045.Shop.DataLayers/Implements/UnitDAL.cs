using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class UnitDAL : IUnitDAL
    {
        private readonly ShopDbContext _context;

        public UnitDAL(ShopDbContext context)
        {
            _context = context;
        }

        public List<Unit> ListUnits(int take = 0)
        {
            var query = _context.Units
                .AsNoTracking()
                .Where(u => !u.IsDeleted)
                .OrderBy(u => u.UnitName);

            if (take > 0)
                return query.Take(take).ToList();

            return query.ToList();
        }

        public Unit? GetById(int id)
        {
            return _context.Units
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == id && !u.IsDeleted);
        }
    }
}
