using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.Abstractions.Models.System;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Services;

public class SystemService : ISystemService
{
    private readonly ISystemDAL _systemDAL;
    private readonly IPasswordHasherService _passwordHasherService;

    public SystemService(ISystemDAL systemDAL, IPasswordHasherService passwordHasherService)
    {
        _systemDAL = systemDAL;
        _passwordHasherService = passwordHasherService;
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
            return Fail("Chỉ có thể chỉnh sửa tài khoản Admin hoặc Staff.");

        var phoneOwner = _systemDAL.GetByPhone(phone);
        if (phoneOwner != null && phoneOwner.Id != input.Id)
            return Fail("Số điện thoại đã được sử dụng bởi tài khoản khác.");

        if (existing.Role == CustomerRoles.Admin &&
            role != CustomerRoles.Admin &&
            _systemDAL.CountActiveAdmins(existing.Id) == 0)
        {
            return Fail("Không thể đổi vai trò của Admin cuối cùng.");
        }

        existing.CustomerName = name;
        existing.Phone = phone;
        existing.Email = NormalizeOptional(input.Email);
        existing.Address = NormalizeOptional(input.Address);
        existing.Role = role;

        var updated = _systemDAL.UpdateAccount(existing);
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
            return Fail("Chỉ có thể đặt lại mật khẩu tài khoản Admin hoặc Staff.");

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
            return Fail("Chỉ có thể quản lý tài khoản Admin hoặc Staff.");

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
        return new SystemRoleUpdateResult
        {
            Success = updated,
            Message = updated ? $"Đã gán vai trò {normalizedRole}." : "Không thể cập nhật vai trò."
        };
    }

    private static string NormalizeBackofficeRole(string? role)
    {
        return string.Equals(role, CustomerRoles.Admin, StringComparison.OrdinalIgnoreCase)
            ? CustomerRoles.Admin
            : CustomerRoles.Staff;
    }

    private static string? NormalizeAssignableRole(string? role)
    {
        if (string.Equals(role, CustomerRoles.Admin, StringComparison.OrdinalIgnoreCase))
            return CustomerRoles.Admin;

        if (string.Equals(role, CustomerRoles.Staff, StringComparison.OrdinalIgnoreCase))
            return CustomerRoles.Staff;

        if (string.Equals(role, CustomerRoles.Customer, StringComparison.OrdinalIgnoreCase))
            return CustomerRoles.Customer;

        return null;
    }

    private static bool IsBackofficeRole(string? role) =>
        string.Equals(role, CustomerRoles.Admin, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(role, CustomerRoles.Staff, StringComparison.OrdinalIgnoreCase);

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static SystemAccountSaveResult Fail(string message) =>
        new() { Success = false, Message = message };

    private static SystemRoleUpdateResult RoleFail(string message) =>
        new() { Success = false, Message = message };
}
