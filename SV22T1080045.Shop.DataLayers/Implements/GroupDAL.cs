using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class GroupDAL : IGroupDAL
    {
        private readonly ShopDbContext _context;

        public GroupDAL(ShopDbContext context)
        {
            _context = context;
        }

        public List<Group> List()
        {
            return _context.Groups
                .AsNoTracking()
                .OrderBy(g => g.Id)
                .ToList();
        }

        public Group? GetById(int id)
        {
            return _context.Groups
                .AsNoTracking()
                .FirstOrDefault(g => g.Id == id);
        }

        public List<Group> ListNonSystem()
        {
            return _context.Groups
                .AsNoTracking()
                .Where(g => !g.IsSystem)
                .OrderBy(g => g.Id)
                .ToList();
        }

        public List<Group> ListWithRoles()
        {
            return _context.Groups
                .AsNoTracking()
                .Include(g => g.RoleGroups)
                .ThenInclude(rg => rg.Role)
                .OrderBy(g => g.Id)
                .ToList();
        }

        public int Add(Group group)
        {
            _context.Groups.Add(group);
            _context.SaveChanges();
            return group.Id;
        }

        public bool Update(Group group)
        {
            var existing = _context.Groups.FirstOrDefault(g => g.Id == group.Id);
            if (existing == null)
                return false;

            existing.Name = group.Name;
            existing.Description = group.Description;
            existing.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var group = _context.Groups.FirstOrDefault(g => g.Id == id);
            if (group == null)
                return false;

            // Không cho xóa group hệ thống
            if (group.IsSystem)
                return false;

            _context.Groups.Remove(group);
            _context.SaveChanges();
            return true;
        }

        public bool IsNameExists(string name, int excludeId = 0)
        {
            return _context.Groups
                .Any(g => g.Name == name && g.Id != excludeId);
        }
    }
}
