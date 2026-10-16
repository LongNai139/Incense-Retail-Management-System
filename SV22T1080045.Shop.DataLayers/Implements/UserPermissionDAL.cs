using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class UserPermissionDAL : IUserPermissionDAL
    {
        private readonly ShopDbContext _context;

        public UserPermissionDAL(ShopDbContext context)
        {
            _context = context;
        }

        public List<UserPermission> GetByUser(int userId)
        {
            return _context.UserPermissions
                .AsNoTracking()
                .Where(up => up.UserId == userId)
                .Include(up => up.Permission)
                .ToList();
        }

        public List<UserPermission> GetByPermission(int permissionId)
        {
            return _context.UserPermissions
                .AsNoTracking()
                .Where(up => up.PermissionId == permissionId)
                .Include(up => up.User)
                .ToList();
        }

        public List<UserPermission> GetActiveByUser(int userId)
        {
            return _context.UserPermissions
                .AsNoTracking()
                .Where(up => up.UserId == userId && up.IsGranted)
                .Where(up => !up.ExpiresAt.HasValue || up.ExpiresAt > DateTime.Now)
                .Include(up => up.Permission)
                .ToList();
        }

        public bool Grant(int userId, int permissionId, int grantedBy, DateTime? expiresAt = null)
        {
            // Kiểm tra đã tồn tại chưa
            var existing = _context.UserPermissions
                .FirstOrDefault(up => up.UserId == userId && up.PermissionId == permissionId);

            if (existing != null)
            {
                // Cập nhật nếu đã tồn tại
                existing.IsGranted = true;
                existing.GrantedAt = DateTime.Now;
                existing.GrantedBy = grantedBy;
                existing.ExpiresAt = expiresAt;
                _context.SaveChanges();
                return true;
            }

            var userPermission = new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId,
                IsGranted = true,
                GrantedAt = DateTime.Now,
                GrantedBy = grantedBy,
                ExpiresAt = expiresAt
            };

            _context.UserPermissions.Add(userPermission);
            _context.SaveChanges();
            return true;
        }

        public bool Revoke(int userId, int permissionId)
        {
            var userPermission = _context.UserPermissions
                .FirstOrDefault(up => up.UserId == userId && up.PermissionId == permissionId);

            if (userPermission == null)
                return false;

            // Thay vì xóa, đánh dấu IsGranted = false để có thể track lịch sử
            userPermission.IsGranted = false;
            userPermission.GrantedAt = DateTime.Now;
            _context.SaveChanges();
            return true;
        }

        public bool ReplaceOverrides(int userId, IReadOnlyCollection<UserPermission> overrides, int grantedBy)
        {
            using var transaction = _context.Database.BeginTransaction();
            var current = _context.UserPermissions.Where(up => up.UserId == userId).ToList();
            var desiredByPermission = overrides.ToDictionary(item => item.PermissionId);
            var currentByPermission = current.ToDictionary(item => item.PermissionId);
            _context.UserPermissions.RemoveRange(current.Where(item => !desiredByPermission.ContainsKey(item.PermissionId)));
            var now = DateTime.Now;
            foreach (var item in desiredByPermission.Values)
            {
                if (currentByPermission.TryGetValue(item.PermissionId, out var existing))
                {
                    existing.IsGranted = item.IsGranted;
                    existing.GrantedAt = now;
                    existing.GrantedBy = grantedBy;
                    existing.ExpiresAt = item.ExpiresAt;
                }
                else
                {
                    _context.UserPermissions.Add(new UserPermission
                    {
                        UserId = userId,
                        PermissionId = item.PermissionId,
                        IsGranted = item.IsGranted,
                        GrantedAt = now,
                        GrantedBy = grantedBy,
                        ExpiresAt = item.ExpiresAt
                    });
                }
            }

            _context.SaveChanges();
            transaction.Commit();
            return true;
        }

        public bool SetOverride(int userId, int permissionId, bool? isGranted, int grantedBy, DateTime? expiresAt = null)
        {
            var existing = _context.UserPermissions
                .FirstOrDefault(up => up.UserId == userId && up.PermissionId == permissionId);

            // null means inherit the role permission, so remove any account-level override.
            if (!isGranted.HasValue)
            {
                if (existing == null)
                    return true;

                _context.UserPermissions.Remove(existing);
                _context.SaveChanges();
                return true;
            }

            if (existing == null)
            {
                existing = new UserPermission
                {
                    UserId = userId,
                    PermissionId = permissionId
                };
                _context.UserPermissions.Add(existing);
            }

            existing.IsGranted = isGranted.Value;
            existing.GrantedAt = DateTime.Now;
            existing.GrantedBy = grantedBy;
            existing.ExpiresAt = expiresAt;
            _context.SaveChanges();
            return true;
        }

        public bool RevokeAll(int userId)
        {
            var userPermissions = _context.UserPermissions
                .Where(up => up.UserId == userId && up.IsGranted)
                .ToList();

            if (!userPermissions.Any())
                return true;

            foreach (var up in userPermissions)
            {
                up.IsGranted = false;
                up.GrantedAt = DateTime.Now;
            }

            _context.SaveChanges();
            return true;
        }

        public bool HasPermission(int userId, int permissionId)
        {
            return _context.UserPermissions
                .Any(up => up.UserId == userId 
                    && up.PermissionId == permissionId 
                    && up.IsGranted
                    && (!up.ExpiresAt.HasValue || up.ExpiresAt > DateTime.Now));
        }

        public int CleanExpired()
        {
            var expiredPermissions = _context.UserPermissions
                .Where(up => up.ExpiresAt.HasValue && up.ExpiresAt < DateTime.Now)
                .ToList();

            if (!expiredPermissions.Any())
                return 0;

            _context.UserPermissions.RemoveRange(expiredPermissions);
            return _context.SaveChanges();
        }
    }
}
