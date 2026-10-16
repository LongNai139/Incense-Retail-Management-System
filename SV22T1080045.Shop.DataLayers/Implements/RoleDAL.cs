using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class RoleDAL : IRoleDAL
    {
        private readonly ShopDbContext _context;

        public RoleDAL(ShopDbContext context)
        {
            _context = context;
        }

        public List<Role> List()
        {
            return _context.Roles
                .AsNoTracking()
                .OrderBy(r => r.Id)
                .ToList();
        }

        public Role? GetById(int id)
        {
            return _context.Roles
                .AsNoTracking()
                .FirstOrDefault(r => r.Id == id);
        }

        public Role? GetByName(string name)
        {
            return _context.Roles
                .AsNoTracking()
                .FirstOrDefault(r => r.Name == name);
        }

        public List<Role> ListNonSystem()
        {
            return _context.Roles
                .AsNoTracking()
                .Where(r => !r.IsSystem)
                .OrderBy(r => r.Id)
                .ToList();
        }

        public int Add(Role role)
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
            return role.Id;
        }

        public bool Update(Role role)
        {
            var existing = _context.Roles.FirstOrDefault(r => r.Id == role.Id);
            if (existing == null)
                return false;

            existing.Name = role.Name;
            existing.DisplayName = role.DisplayName;
            existing.Description = role.Description;
            existing.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
                return false;

            // Không cho xóa role hệ thống
            if (role.IsSystem)
                return false;

            _context.Roles.Remove(role);
            _context.SaveChanges();
            return true;
        }

        public bool IsNameExists(string name, int excludeId = 0)
        {
            return _context.Roles
                .Any(r => r.Name == name && r.Id != excludeId);
        }
    }
}
