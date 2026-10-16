using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.Abstractions.Models.System;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Services;

public class SystemService : ISystemService
{
    private readonly ISystemDAL _systemDAL;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IPermissionService _permissionService;
    private readonly IRoleDAL _roleDAL;

    public SystemService(ISystemDAL systemDAL, IPasswordHasherService passwordHasherService, IPermissionService permissionService, IRoleDAL roleDAL)
    {
        _systemDAL = systemDAL;
        _passwordHasherService = passwordHasherService;
        _permissionService = permissionService;
        _roleDAL = roleDAL;
    }

    public List<SystemAccountData> ListBackofficeAccounts(string? searchValue, string? roleFilter) =>
        _systemDAL.ListBackofficeAccounts(searchValue, roleFilter);

    public List<SystemAccountData> ListAccountsForRoleAssignment(string? searchValue, string? roleFilter) =>
        _systemDAL.ListAccountsForRoleAssignment(searchValue, roleFilter);

    public SystemAccountSaveResult SaveBackofficeAccount(SystemAccountInput input, int actorId)
    {
        var name = input.CustomerName?.Trim() ?? "";
        var phone = input.Phone?.Trim() ?? "";
        var role = NormalizeBackofficeRole(input.Role);
        if (role == null)
            return Fail("Vai trò không hợp lệ hoặc không thể gán cho tài khoản quản trị.");
        if (string.Equals(role, CustomerRoles.Admin, StringComparison.OrdinalIgnoreCase) && !IsAdmin(actorId))
            return Fail("Chỉ Admin hiện tại mới được gán vai trò Admin.");

        if (string.IsNullOrWhiteSpace(name))
            return Fail("Vui lòng nhập họ tên.");

        if (string.IsNullOrWhiteSpace(phone))
            return Fail("Vui lòng nhập số điện thoại.");

        if (input.Id <= 0)
        {
            if (string.IsNullOrWhiteSpace(input.Password))
                return Fail("Vui lòng nhập mật khẩu cho tài khoản mới.");

            if (_systemDAL.GetByPhone(phone) != null)
                return Fail("Số điện thoại đã được sử dụng.");

            var account = new Customer
            {
                CustomerName = name,
                Phone = phone,
                Email = NormalizeOptional(input.Email),
                Address = NormalizeOptional(input.Address),
                Role = role,
                Password = _passwordHasherService.Hash(input.Password!),
                CreatedTime = DateTime.Now,
                IsDeleted = false
            };

            // Tự động gán RoleId tương ứng với Role string
            // (Sẽ được DAL gán khi thêm vào DB, nhưng có thể query ở đây nếu cần)

            var id = _systemDAL.AddAccount(account);
            return new SystemAccountSaveResult
            {
                Success = id > 0,
                AccountId = id,
                Message = id > 0 ? "Đã tạo tài khoản quản trị." : "Không thể tạo tài khoản."
            };
        }

        var existing = _systemDAL.GetAccountById(input.Id);
        if (existing == null)
            return Fail("Không tìm thấy tài khoản.");

        if (!IsBackofficeRole(existing.Role))
            return Fail("Không thể chỉnh sửa tài khoản với vai trò Customer.");

        if (input.Id == actorId && !string.Equals(role, CustomerRoles.Admin, StringComparison.OrdinalIgnoreCase))
            return Fail("Không thể tự hạ quyền của chính mình.");

        var phoneOwner = _systemDAL.GetByPhone(phone);
        if (phoneOwner != null && phoneOwner.Id != input.Id)
            return Fail("Số điện thoại đã được sử dụng bởi tài khoản khác.");

        if (existing.Role == CustomerRoles.Admin &&
            role != CustomerRoles.Admin &&
            _systemDAL.CountActiveAdmins(existing.Id) == 0)
        {
            return Fail("Không thể đổi vai trò của Admin cuối cùng.");
        }

        var roleChanged = !string.Equals(existing.Role, role, StringComparison.OrdinalIgnoreCase);
        existing.CustomerName = name;
        existing.Phone = phone;
        existing.Email = NormalizeOptional(input.Email);
        existing.Address = NormalizeOptional(input.Address);
        existing.Role = role;

        var updated = _systemDAL.UpdateAccount(existing);
        if (updated && roleChanged)
            _permissionService.InvalidateUser(existing.Id);

        return new SystemAccountSaveResult
        {
            Success = updated,
            AccountId = existing.Id,
            Message = updated ? "Đã cập nhật tài khoản." : "Không thể cập nhật tài khoản."
        };
    }

    public SystemAccountSaveResult ResetPassword(int accountId, string newPassword, int actorId)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
            return Fail("Vui lòng nhập mật khẩu mới.");

        var existing = _systemDAL.GetAccountById(accountId);
        if (existing == null)
            return Fail("Không tìm thấy tài khoản.");

        if (!IsBackofficeRole(existing.Role))
            return Fail("Không thể đặt lại mật khẩu tài khoản với vai trò Customer.");

        existing.Password = _passwordHasherService.Hash(newPassword.Trim());
        var updated = _systemDAL.UpdateAccount(existing);

        return new SystemAccountSaveResult
        {
            Success = updated,
            AccountId = accountId,
            Message = updated ? "Đã đặt lại mật khẩu." : "Không thể đặt lại mật khẩu."
        };
    }

    public SystemAccountSaveResult SetAccountActive(int accountId, bool isActive, int actorId)
    {
        if (accountId == actorId && !isActive)
            return Fail("Không thể vô hiệu hóa tài khoản đang đăng nhập.");

        var existing = _systemDAL.GetAccountById(accountId);
        if (existing == null)
            return Fail("Không tìm thấy tài khoản.");

        if (!IsBackofficeRole(existing.Role))
            return Fail("Không thể quản lý trạng thái tài khoản với vai trò Customer.");

        if (!isActive &&
            existing.Role == CustomerRoles.Admin &&
            _systemDAL.CountActiveAdmins(existing.Id) == 0)
        {
            return Fail("Không thể vô hiệu hóa Admin cuối cùng.");
        }

        var updated = _systemDAL.SetDeleted(accountId, !isActive);
        return new SystemAccountSaveResult
        {
            Success = updated,
            AccountId = accountId,
            Message = updated
                ? isActive ? "Đã kích hoạt lại tài khoản." : "Đã vô hiệu hóa tài khoản."
                : "Không thể cập nhật trạng thái tài khoản."
        };
    }

    public SystemRoleUpdateResult UpdateAccountRole(int accountId, string role, int actorId)
    {
        var normalizedRole = NormalizeAssignableRole(role);
        if (normalizedRole == null)
            return RoleFail("Vai trò không hợp lệ.");
        if (string.Equals(normalizedRole, CustomerRoles.Admin, StringComparison.OrdinalIgnoreCase) && !IsAdmin(actorId))
            return RoleFail("Chỉ Admin hiện tại mới được gán vai trò Admin.");

        var existing = _systemDAL.GetAccountById(accountId);
        if (existing == null)
            return RoleFail("Không tìm thấy tài khoản.");

        if (accountId == actorId && normalizedRole != CustomerRoles.Admin)
            return RoleFail("Không thể tự hạ quyền của chính mình.");

        if (existing.Role == CustomerRoles.Admin &&
            normalizedRole != CustomerRoles.Admin &&
            _systemDAL.CountActiveAdmins(existing.Id) == 0)
        {
            return RoleFail("Không thể hạ quyền Admin cuối cùng.");
        }

        var updated = _systemDAL.UpdateRole(accountId, normalizedRole);

        // Invalidate cache permission của user bị thay đổi role
        if (updated)
        {
            _permissionService.InvalidateUser(accountId);
        }

        return new SystemRoleUpdateResult
        {
            Success = updated,
            Message = updated ? $"Đã gán vai trò {normalizedRole}." : "Không thể cập nhật vai trò."
        };
    }

    private string? NormalizeBackofficeRole(string? role)
    {
        var value = role?.Trim();
        var matched = value == null ? null : _roleDAL.GetByName(value);
        return matched == null || string.Equals(matched.Name, CustomerRoles.Customer, StringComparison.OrdinalIgnoreCase)
            ? null
            : matched.Name;
    }

    private string? NormalizeAssignableRole(string? role)
    {
        return NormalizeBackofficeRole(role);
    }

    private bool IsBackofficeRole(string? role) => NormalizeBackofficeRole(role) != null;

    private bool IsAdmin(int userId)
    {
        var actor = _systemDAL.GetAccountById(userId);
        return actor != null && string.Equals(actor.Role, CustomerRoles.Admin, StringComparison.OrdinalIgnoreCase);
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static SystemAccountSaveResult Fail(string message) =>
        new() { Success = false, Message = message };

    private static SystemRoleUpdateResult RoleFail(string message) =>
        new() { Success = false, Message = message };
}
