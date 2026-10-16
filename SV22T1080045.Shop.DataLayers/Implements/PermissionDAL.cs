using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class PermissionDAL : IPermissionDAL
    {
        private readonly ShopDbContext _context;

        public PermissionDAL(ShopDbContext context)
        {
            _context = context;
        }

        public List<Permission> List()
        {
            return _context.Permissions
                .AsNoTracking()
                .OrderBy(p => p.Module)
                .ThenBy(p => p.Code)
                .ToList();
        }

        public Permission? GetById(int id)
        {
            return _context.Permissions
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id);
        }

        public Permission? GetByCode(string code)
        {
            return _context.Permissions
                .AsNoTracking()
                .FirstOrDefault(p => p.Code == code);
        }

        public List<Permission> GetByModule(string module)
        {
            return _context.Permissions
                .AsNoTracking()
                .Where(p => p.Module == module)
                .OrderBy(p => p.Code)
                .ToList();
        }

        public List<Permission> GetByRole(int roleId)
        {
            return _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.Permission!)
                .AsNoTracking()
                .OrderBy(p => p.Module)
                .ThenBy(p => p.Code)
                .ToList();
        }

        public List<Permission> GetByUser(int userId)
        {
            var now = DateTime.Now;

            // Permission lấy từ role của user (không dùng toán tử ?: trong SelectMany để EF dịch được sang SQL)
            var rolePermIds = _context.RolePermissions
                .Where(rp => _context.Customers
                    .Any(c => c.Id == userId && c.RoleId == rp.RoleId))
                .Select(rp => rp.PermissionId);

            // Permission cấp riêng cho user, còn hiệu lực
            var grantedIds = _context.UserPermissions
                .Where(up => up.UserId == userId && up.IsGranted
                          && (up.ExpiresAt == null || up.ExpiresAt > now))
                .Select(up => up.PermissionId);

            // Permission bị thu hồi riêng (IsGranted = false) => ghi đè quyền từ role
            var deniedIds = _context.UserPermissions
                .Where(up => up.UserId == userId && !up.IsGranted
                          && (up.ExpiresAt == null || up.ExpiresAt > now))
                .Select(up => up.PermissionId);

            // Một câu query duy nhất, DB tự loại trùng
            return _context.Permissions
                .AsNoTracking()
                .Where(p => (rolePermIds.Contains(p.Id) || grantedIds.Contains(p.Id))
                         && !deniedIds.Contains(p.Id))
                .OrderBy(p => p.Module)
                .ThenBy(p => p.Code)
                .ToList();
        }

        public int Add(Permission permission)
        {
            _context.Permissions.Add(permission);
            _context.SaveChanges();
            return permission.Id;
        }

        public bool Update(Permission permission)
        {
            var existing = _context.Permissions.FirstOrDefault(p => p.Id == permission.Id);
            if (existing == null)
                return false;

            existing.Code = permission.Code;
            existing.DisplayName = permission.DisplayName;
            existing.Description = permission.Description;
            existing.Module = permission.Module;

            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var permission = _context.Permissions.FirstOrDefault(p => p.Id == id);
            if (permission == null)
                return false;

            _context.Permissions.Remove(permission);
            _context.SaveChanges();
            return true;
        }

        public bool IsCodeExists(string code, int excludeId = 0)
        {
            return _context.Permissions
                .Any(p => p.Code == code && p.Id != excludeId);
        }
    }
}