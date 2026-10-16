using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Interfaces;

public interface IPermissionAdministrationService
{
    List<Role> ListRoles();
    List<Permission> ListPermissions();
    List<Customer> ListUsers();
    HashSet<int> GetRolePermissionIds(int roleId);
    Dictionary<int, UserPermission> GetUserOverrides(int userId);
    bool SaveRole(Role role, out string message);
    bool DeleteRole(int roleId, out string message);
    bool SavePermission(Permission permission, out string message);
    bool DeletePermission(int permissionId, out string message);
    bool SaveRolePermissions(int roleId, IReadOnlyCollection<int> permissionIds, int actorId, out string message);
    bool SaveUserOverrides(int userId, IReadOnlyCollection<UserPermission> overrides, int actorId, out string message);
}
