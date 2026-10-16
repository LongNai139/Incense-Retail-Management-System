using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class RolePermissionDAL : IRolePermissionDAL
    {
        private readonly ShopDbContext _context;

        public RolePermissionDAL(ShopDbContext context)
        {
            _context = context;
        }

        public List<RolePermission> GetByRole(int roleId)
        {
            return _context.RolePermissions
                .AsNoTracking()
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Permission)
                .ToList();
        }

        public List<RolePermission> GetByPermission(int permissionId)
        {
            return _context.RolePermissions
                .AsNoTracking()
                .Where(rp => rp.PermissionId == permissionId)
                .Include(rp => rp.Role)
                .ToList();
        }

        public bool Grant(int roleId, int permissionId, int grantedBy)
        {
            // Kiểm tra đã tồn tại chưa
            if (HasPermission(roleId, permissionId))
                return true; // Đã có rồi

            var rolePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
                GrantedAt = DateTime.Now,
                GrantedBy = grantedBy
            };

            _context.RolePermissions.Add(rolePermission);
            _context.SaveChanges();
            return true;
        }

        public bool Revoke(int roleId, int permissionId)
        {
            var rolePermission = _context.RolePermissions
                .FirstOrDefault(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

            if (rolePermission == null)
                return false;

            _context.RolePermissions.Remove(rolePermission);
            _context.SaveChanges();
            return true;
        }

        public bool RevokeAll(int roleId)
        {
            var rolePermissions = _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToList();

            if (!rolePermissions.Any())
                return true;

            _context.RolePermissions.RemoveRange(rolePermissions);
            _context.SaveChanges();
            return true;
        }

        public bool ReplaceAll(int roleId, IReadOnlyCollection<int> permissionIds, int grantedBy)
        {
            using var transaction = _context.Database.BeginTransaction();
            var current = _context.RolePermissions.Where(rp => rp.RoleId == roleId).ToList();
            var desiredIds = permissionIds.Distinct().ToHashSet();
            var currentById = current.ToDictionary(item => item.PermissionId);
            _context.RolePermissions.RemoveRange(current.Where(item => !desiredIds.Contains(item.PermissionId)));
            var now = DateTime.Now;
            foreach (var permissionId in desiredIds)
            {
                if (currentById.TryGetValue(permissionId, out var existing))
                {
                    existing.GrantedAt = now;
                    existing.GrantedBy = grantedBy;
                }
                else
                {
                    _context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = permissionId,
                        GrantedAt = now,
                        GrantedBy = grantedBy
                    });
                }
            }

            _context.SaveChanges();
            transaction.Commit();
            return true;
        }

        public bool HasPermission(int roleId, int permissionId)
        {
            return _context.RolePermissions
                .Any(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
        }
    }
}
