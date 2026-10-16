using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class RoleGroupDAL : IRoleGroupDAL
    {
        private readonly ShopDbContext _context;

        public RoleGroupDAL(ShopDbContext context)
        {
            _context = context;
        }

        public List<RoleGroup> GetByGroup(int groupId)
        {
            return _context.RoleGroups
                .AsNoTracking()
                .Where(rg => rg.GroupId == groupId)
                .Include(rg => rg.Role)
                .ToList();
        }

        public List<RoleGroup> GetByRole(int roleId)
        {
            return _context.RoleGroups
                .AsNoTracking()
                .Where(rg => rg.RoleId == roleId)
                .Include(rg => rg.Group)
                .ToList();
        }

        public bool Assign(int groupId, int roleId)
        {
            // Kiểm tra đã tồn tại chưa
            if (HasRole(groupId, roleId))
                return true; // Đã có rồi

            var roleGroup = new RoleGroup
            {
                GroupId = groupId,
                RoleId = roleId
            };

            _context.RoleGroups.Add(roleGroup);
            _context.SaveChanges();
            return true;
        }

        public bool AssignMany(int groupId, List<int> roleIds)
        {
            if (roleIds == null || !roleIds.Any())
                return true;

            foreach (var roleId in roleIds)
            {
                if (!HasRole(groupId, roleId))
                {
                    _context.RoleGroups.Add(new RoleGroup
                    {
                        GroupId = groupId,
                        RoleId = roleId
                    });
                }
            }

            _context.SaveChanges();
            return true;
        }

        public bool Remove(int groupId, int roleId)
        {
            var roleGroup = _context.RoleGroups
                .FirstOrDefault(rg => rg.GroupId == groupId && rg.RoleId == roleId);

            if (roleGroup == null)
                return false;

            _context.RoleGroups.Remove(roleGroup);
            _context.SaveChanges();
            return true;
        }

        public bool RemoveAll(int groupId)
        {
            var roleGroups = _context.RoleGroups
                .Where(rg => rg.GroupId == groupId)
                .ToList();

            if (!roleGroups.Any())
                return true;

            _context.RoleGroups.RemoveRange(roleGroups);
            _context.SaveChanges();
            return true;
        }

        public bool HasRole(int groupId, int roleId)
        {
            return _context.RoleGroups
                .Any(rg => rg.GroupId == groupId && rg.RoleId == roleId);
        }
    }
}
