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

        public int Add(Unit unit)
        {
            _context.Units.Add(unit);
            _context.SaveChanges();
            return unit.Id;
        }

        public bool Update(Unit unit)
        {
            _context.Units.Update(unit);
            return _context.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var unit = _context.Units.FirstOrDefault(u => u.Id == id && !u.IsDeleted);
            if (unit == null)
                return false;

            unit.IsDeleted = true;
            return _context.SaveChanges() > 0;
        }
    }
}
