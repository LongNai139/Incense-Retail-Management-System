using System.Reflection;
using System.Text.RegularExpressions;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Services;

public sealed class PermissionAdministrationService : IPermissionAdministrationService
{
    private static readonly HashSet<string> ApplicationPermissionCodes = typeof(PermissionConstants)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(field => field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
        .Select(field => (string)field.GetRawConstantValue()!)
        .ToHashSet(StringComparer.OrdinalIgnoreCase);

    private static readonly Regex PermissionCodePattern = new("^[a-z][a-z0-9_]{1,99}$", RegexOptions.Compiled);

    private readonly IRoleDAL _roleDAL;
    private readonly IPermissionDAL _permissionDAL;
    private readonly IRolePermissionDAL _rolePermissionDAL;
    private readonly IUserPermissionDAL _userPermissionDAL;
    private readonly ICustomerDAL _customerDAL;
    private readonly IPermissionService _permissionService;

    public PermissionAdministrationService(
        IRoleDAL roleDAL,
        IPermissionDAL permissionDAL,
        IRolePermissionDAL rolePermissionDAL,
        IUserPermissionDAL userPermissionDAL,
        ICustomerDAL customerDAL,
        IPermissionService permissionService)
    {
        _roleDAL = roleDAL;
        _permissionDAL = permissionDAL;
        _rolePermissionDAL = rolePermissionDAL;
        _userPermissionDAL = userPermissionDAL;
        _customerDAL = customerDAL;
        _permissionService = permissionService;
    }

    public List<Role> ListRoles() => _roleDAL.List();
    public List<Permission> ListPermissions() => _permissionDAL.List();
    public List<Customer> ListUsers() => _customerDAL.ListForPermissionAssignment();

    public HashSet<int> GetRolePermissionIds(int roleId) => _rolePermissionDAL.GetByRole(roleId)
        .Select(item => item.PermissionId)
        .ToHashSet();

    public Dictionary<int, UserPermission> GetUserOverrides(int userId) => _userPermissionDAL.GetByUser(userId)
        .ToDictionary(item => item.PermissionId);

    public bool SaveRole(Role role, out string message)
    {
        role.Name = role.Name?.Trim() ?? "";
        role.DisplayName = role.DisplayName?.Trim() ?? "";
        role.Description = string.IsNullOrWhiteSpace(role.Description) ? null : role.Description.Trim();

        if (role.Name.Length is < 2 or > 50 || role.DisplayName.Length is < 2 or > 100)
            return Fail("Tên Role cần 2–50 ký tự và tên hiển thị cần 2–100 ký tự.", out message);

        if (_roleDAL.IsNameExists(role.Name, role.Id))
            return Fail("Tên Role đã tồn tại.", out message);

        if (role.Id <= 0)
        {
            role.IsSystem = false;
            role.CreatedAt = DateTime.Now;
            role.UpdatedAt = role.CreatedAt;
            if (_roleDAL.Add(role) <= 0)
                return Fail("Không thể tạo Role.", out message);
        }
        else
        {
            var existing = _roleDAL.GetById(role.Id);
            if (existing == null)
                return Fail("Không tìm thấy Role.", out message);
            if (!string.Equals(existing.Name, role.Name, StringComparison.Ordinal))
                return Fail("Mã Role không thể đổi sau khi tạo; hãy sửa tên hiển thị.", out message);

            if (!_roleDAL.Update(role))
                return Fail("Không thể cập nhật Role.", out message);
        }

        message = "Đã lưu Role.";
        return true;
    }

    public bool DeleteRole(int roleId, out string message)
    {
        var role = _roleDAL.GetById(roleId);
        if (role == null)
            return Fail("Không tìm thấy Role.", out message);
        if (role.IsSystem)
            return Fail("Không thể xóa Role hệ thống.", out message);
        if (_customerDAL.ListForPermissionAssignment().Any(user => user.RoleId == roleId || user.Role == role.Name))
            return Fail("Role đang được gán cho tài khoản; hãy chuyển các tài khoản trước khi xóa.", out message);
        if (!_roleDAL.Delete(roleId))
            return Fail("Không thể xóa Role.", out message);

        _permissionService.InvalidateAll();
        message = "Đã xóa Role.";
        return true;
    }

    public bool SavePermission(Permission permission, out string message)
    {
        permission.Code = permission.Code?.Trim().ToLowerInvariant() ?? "";
        permission.DisplayName = permission.DisplayName?.Trim() ?? "";
        permission.Description = string.IsNullOrWhiteSpace(permission.Description) ? null : permission.Description.Trim();
        permission.Module = string.IsNullOrWhiteSpace(permission.Module) ? null : permission.Module.Trim();

        if (!PermissionCodePattern.IsMatch(permission.Code) || permission.DisplayName.Length is < 2 or > 100)
            return Fail("Mã quyền phải theo dạng action_resource; tên hiển thị cần 2–100 ký tự.", out message);
        if (permission.Description?.Length > 500 || permission.Module?.Length > 50)
            return Fail("Mô tả tối đa 500 ký tự và module tối đa 50 ký tự.", out message);
        if (_permissionDAL.IsCodeExists(permission.Code, permission.Id))
            return Fail("Mã quyền đã tồn tại.", out message);

        if (permission.Id <= 0)
        {
            if (_permissionDAL.Add(permission) <= 0)
                return Fail("Không thể tạo quyền.", out message);
        }
        else
        {
            var existing = _permissionDAL.GetById(permission.Id);
            if (existing == null)
                return Fail("Không tìm thấy quyền.", out message);
            if (ApplicationPermissionCodes.Contains(existing.Code) &&
                !string.Equals(existing.Code, permission.Code, StringComparison.OrdinalIgnoreCase))
                return Fail("Không thể đổi mã quyền đang được ứng dụng sử dụng.", out message);
            if (!_permissionDAL.Update(permission))
                return Fail("Không thể cập nhật quyền.", out message);
        }

        _permissionService.InvalidateAll();
        message = "Đã lưu quyền.";
        return true;
    }

    public bool DeletePermission(int permissionId, out string message)
    {
        var permission = _permissionDAL.GetById(permissionId);
        if (permission == null)
            return Fail("Không tìm thấy quyền.", out message);
        if (ApplicationPermissionCodes.Contains(permission.Code))
            return Fail("Không thể xóa mã quyền đang được ứng dụng sử dụng.", out message);
        if (!_permissionDAL.Delete(permissionId))
            return Fail("Không thể xóa quyền.", out message);

        _permissionService.InvalidateAll();
        message = "Đã xóa quyền.";
        return true;
    }

    public bool SaveRolePermissions(int roleId, IReadOnlyCollection<int> permissionIds, int actorId, out string message)
    {
        var role = _roleDAL.GetById(roleId);
        if (role == null)
            return Fail("Không tìm thấy Role.", out message);
        if (string.Equals(role.Name, CustomerRoles.Admin, StringComparison.OrdinalIgnoreCase))
            return Fail("Admin luôn có toàn quyền theo cấu hình hệ thống.", out message);

        var allowedIds = _permissionDAL.List().Select(item => item.Id).ToHashSet();
        var selectedIds = permissionIds.Distinct().ToArray();
        if (selectedIds.Any(id => !allowedIds.Contains(id)))
            return Fail("Danh sách quyền gửi lên không hợp lệ.", out message);
        if (!_rolePermissionDAL.ReplaceAll(roleId, selectedIds, actorId))
            return Fail("Không thể lưu quyền cho Role.", out message);

        _permissionService.InvalidateAll();
        message = "Đã cập nhật quyền cho Role. Người dùng cần đăng nhập lại để nhận quyền mới.";
        return true;
    }

    public bool SaveUserOverrides(int userId, IReadOnlyCollection<UserPermission> overrides, int actorId, out string message)
    {
        var user = _customerDAL.GetById(userId);
        if (user == null)
            return Fail("Tài khoản hoặc quyền không hợp lệ.", out message);
        if (string.Equals(user.Role, CustomerRoles.Admin, StringComparison.OrdinalIgnoreCase))
            return Fail("Admin luôn có toàn quyền; không thể ghi đè quyền riêng cho tài khoản này.", out message);
        var permissionIds = overrides.Select(item => item.PermissionId).ToArray();
        var allowedPermissionIds = _permissionDAL.List().Select(item => item.Id).ToHashSet();
        if (permissionIds.Distinct().Count() != permissionIds.Length ||
            permissionIds.Any(id => !allowedPermissionIds.Contains(id)))
            return Fail("Danh sách quyền không hợp lệ.", out message);
        if (overrides.Any(item => item.ExpiresAt.HasValue && item.ExpiresAt.Value <= DateTime.Now))
            return Fail("Thời hạn quyền phải nằm trong tương lai.", out message);
        if (!_userPermissionDAL.ReplaceOverrides(userId, overrides, actorId))
            return Fail("Không thể lưu quyền riêng cho tài khoản.", out message);

        _permissionService.InvalidateUser(userId);
        message = "Đã cập nhật quyền riêng. Người dùng cần đăng nhập lại để nhận thay đổi.";
        return true;
    }

    private static bool Fail(string text, out string message)
    {
        message = text;
        return false;
    }
}
