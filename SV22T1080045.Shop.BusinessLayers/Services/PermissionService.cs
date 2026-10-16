using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.BusinessLayers.Interfaces;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class PermissionService : IPermissionService
    {
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

        // Token dùng chung (static) để xóa cache của tất cả user cùng lúc.
        // IMemoryCache là singleton nên các instance PermissionService (scoped) dùng chung cache này.
        private static CancellationTokenSource _resetTokenSource = new();

        private readonly IPermissionDAL _permissionDAL;
        private readonly IMemoryCache _cache;

        public PermissionService(IPermissionDAL permissionDAL, IMemoryCache cache)
        {
            _permissionDAL = permissionDAL;
            _cache = cache;
        }

        private static string UserKey(int userId) => $"permissions:user:{userId}";

        public IReadOnlySet<string> GetUserPermissionCodes(int userId)
        {
            if (userId <= 0)
                return new HashSet<string>();

            if (_cache.TryGetValue(UserKey(userId), out HashSet<string>? cached) && cached != null)
                return cached;

            var codes = _permissionDAL.GetByUser(userId)
                .Select(p => p.Code)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(CacheDuration)
                .AddExpirationToken(new CancellationChangeToken(Volatile.Read(ref _resetTokenSource).Token));

            _cache.Set(UserKey(userId), codes, options);
            return codes;
        }

        public bool HasPermission(int userId, string permissionCode)
        {
            if (string.IsNullOrWhiteSpace(permissionCode))
                return false;

            return GetUserPermissionCodes(userId).Contains(permissionCode.Trim());
        }

        public void InvalidateUser(int userId)
        {
            _cache.Remove(UserKey(userId));
        }

        public void InvalidateAll()
        {
            var old = Interlocked.Exchange(ref _resetTokenSource, new CancellationTokenSource());
            old.Cancel();
            old.Dispose();
        }
    }
}