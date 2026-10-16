using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.Admin.Extensions.Authorizations;
using SV22T1080045.Shop.Abstractions.Models.System;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.Models.ViewModels.System;
using System.Security.Claims;

namespace SV22T1080045.Shop.Controllers;

[Authorize]
public class SystemController : Controller
{
    private readonly ISystemService _systemService;
    private readonly IPermissionAdministrationService _permissionAdministrationService;

    public SystemController(ISystemService systemService, IPermissionAdministrationService permissionAdministrationService)
    {
        _systemService = systemService;
        _permissionAdministrationService = permissionAdministrationService;
    }

    [Permission(PermissionConstants.ManageSystem, PermissionConstants.ManagePermission, PermissionConstants.ViewUser)]
    public IActionResult Index(
        string accountSearchValue = "",
        string accountRoleFilter = "",
        string roleSearchValue = "",
        string roleFilter = "",
        int? editAccountId = null,
        int? permissionRoleId = null,
        int? permissionUserId = null)
    {
        return View(BuildDashboardModel(
            accountSearchValue,
            accountRoleFilter,
            roleSearchValue,
            roleFilter,
            editAccountId,
            permissionRoleId,
            permissionUserId));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SaveAccount([Bind(Prefix = "AccountForm")] SystemAccountFormViewModel form)
    {
        var requiredPermission = form.Id > 0 ? PermissionConstants.EditUser : PermissionConstants.CreateUser;
        if (!User.HasAnyPermission(requiredPermission, PermissionConstants.ManageSystem))
            return Forbid();

        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index), null, new { editAccountId = form.Id }, "accounts");
        }

        var result = _systemService.SaveBackofficeAccount(new SystemAccountInput
        {
            Id = form.Id,
            CustomerName = form.CustomerName,
            Phone = form.Phone,
            Email = form.Email,
            Address = form.Address,
            Role = form.Role,
            Password = form.Password
        }, GetCurrentUserId());

        TempData["SystemMessage"] = result.Message;
        TempData["SystemMessageSuccess"] = result.Success;
        return RedirectToAction(nameof(Index), null, new
        {
            editAccountId = result.Success && form.Id <= 0 ? result.AccountId : form.Id > 0 ? form.Id : (int?)null
        }, "accounts");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission(PermissionConstants.ResetPassword, PermissionConstants.ManageSystem)]
    public IActionResult ResetPassword(int accountId, string newPassword)
    {
        var result = _systemService.ResetPassword(accountId, newPassword, GetCurrentUserId());
        TempData["SystemMessage"] = result.Message;
        TempData["SystemMessageSuccess"] = result.Success;
        return RedirectToAction(nameof(Index), null, new { editAccountId = accountId }, "accounts");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission(PermissionConstants.ManagePermission, PermissionConstants.ManageSystem)]
    public IActionResult UpdateRole(int accountId, string role)
    {
        var result = _systemService.UpdateAccountRole(accountId, role, GetCurrentUserId());
        TempData["SystemMessage"] = result.Message;
        TempData["SystemMessageSuccess"] = result.Success;
        return RedirectToAction(nameof(Index), null, null, "permissions");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission(PermissionConstants.ManagePermission, PermissionConstants.ManageSystem)]
    public IActionResult SaveRolePermissions(int roleId, List<int>? permissionIds)
    {
        var success = _permissionAdministrationService.SaveRolePermissions(
            roleId, permissionIds ?? new List<int>(), GetCurrentUserId(), out var message);
        TempData["SystemMessage"] = message;
        TempData["SystemMessageSuccess"] = success;
        return RedirectToAction(nameof(Index), null, new { permissionRoleId = roleId }, "permissions");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission(PermissionConstants.ManagePermission, PermissionConstants.ManageSystem)]
    public IActionResult SaveUserPermissions(int userId, List<SystemUserPermissionInputViewModel>? permissions)
    {
        var overrides = new List<UserPermission>();
        foreach (var item in permissions ?? new List<SystemUserPermissionInputViewModel>())
        {
            bool isGranted;
            switch (item.Mode)
            {
                case "Grant":
                    isGranted = true;
                    break;
                case "Deny":
                    isGranted = false;
                    break;
                case "Inherit":
                    continue;
                default:
                    TempData["SystemMessage"] = "Có lựa chọn quyền riêng không hợp lệ.";
                    TempData["SystemMessageSuccess"] = false;
                    return RedirectToAction(nameof(Index), null, new { permissionUserId = userId }, "permissions");
            }

            overrides.Add(new UserPermission
            {
                PermissionId = item.PermissionId,
                IsGranted = isGranted,
                ExpiresAt = item.ExpiresAt
            });
        }

        var success = _permissionAdministrationService.SaveUserOverrides(
            userId, overrides, GetCurrentUserId(), out var message);
        TempData["SystemMessage"] = message;
        TempData["SystemMessageSuccess"] = success;
        return RedirectToAction(nameof(Index), null, new { permissionUserId = userId }, "permissions");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission(PermissionConstants.ManagePermission, PermissionConstants.ManageSystem)]
    public IActionResult SaveRole(SystemRoleFormViewModel form)
    {
        var success = _permissionAdministrationService.SaveRole(new Role
        {
            Id = form.Id,
            Name = form.Name,
            DisplayName = form.DisplayName,
            Description = form.Description
        }, out var message);
        TempData["SystemMessage"] = message;
        TempData["SystemMessageSuccess"] = success;
        return RedirectToAction(nameof(Index), null, null, "permissions");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission(PermissionConstants.ManagePermission, PermissionConstants.ManageSystem)]
    public IActionResult DeleteRole(int roleId)
    {
        var success = _permissionAdministrationService.DeleteRole(roleId, out var message);
        TempData["SystemMessage"] = message;
        TempData["SystemMessageSuccess"] = success;
        return RedirectToAction(nameof(Index), null, null, "permissions");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission(PermissionConstants.ManagePermission, PermissionConstants.ManageSystem)]
    public IActionResult SavePermission(SystemPermissionFormViewModel form)
    {
        var success = _permissionAdministrationService.SavePermission(new Permission
        {
            Id = form.Id,
            Code = form.Code,
            DisplayName = form.DisplayName,
            Description = form.Description,
            Module = form.Module
        }, out var message);
        TempData["SystemMessage"] = message;
        TempData["SystemMessageSuccess"] = success;
        return RedirectToAction(nameof(Index), null, null, "permission-catalog");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Permission(PermissionConstants.ManagePermission, PermissionConstants.ManageSystem)]
    public IActionResult DeletePermission(int permissionId)
    {
        var success = _permissionAdministrationService.DeletePermission(permissionId, out var message);
        TempData["SystemMessage"] = message;
        TempData["SystemMessageSuccess"] = success;
        return RedirectToAction(nameof(Index), null, null, "permission-catalog");
    }

    private SystemDashboardViewModel BuildDashboardModel(
        string accountSearchValue,
        string accountRoleFilter,
        string roleSearchValue,
        string roleFilter,
        int? editAccountId,
        int? permissionRoleId,
        int? permissionUserId)
    {
        var currentUserId = GetCurrentUserId();
        var backofficeAccounts = _systemService
            .ListBackofficeAccounts(accountSearchValue, accountRoleFilter)
            .Select(a => ToRow(a, currentUserId))
            .ToList();

        var roleAccounts = _systemService
            .ListAccountsForRoleAssignment(roleSearchValue, roleFilter)
            .Select(a => ToRow(a, currentUserId))
            .ToList();

        var roles = _permissionAdministrationService.ListRoles();
        var selectedRoleId = permissionRoleId is > 0 && roles.Any(role => role.Id == permissionRoleId)
            ? permissionRoleId.Value
            : roles.FirstOrDefault(role => role.Name == CustomerRoles.Staff)?.Id
              ?? roles.FirstOrDefault()?.Id
              ?? 0;
        var users = _permissionAdministrationService.ListUsers();
        var selectedUserId = permissionUserId is > 0 && users.Any(user => user.Id == permissionUserId)
            ? permissionUserId.Value
            : users.FirstOrDefault(user => user.Id == currentUserId)?.Id
              ?? users.FirstOrDefault()?.Id
              ?? 0;
        var permissions = _permissionAdministrationService.ListPermissions();
        var rolePermissionIds = selectedRoleId > 0
            ? _permissionAdministrationService.GetRolePermissionIds(selectedRoleId)
            : new HashSet<int>();
        var userOverrides = selectedUserId > 0
            ? _permissionAdministrationService.GetUserOverrides(selectedUserId)
            : new Dictionary<int, UserPermission>();

        return new SystemDashboardViewModel
        {
            AccountSearchValue = accountSearchValue?.Trim() ?? "",
            AccountRoleFilter = accountRoleFilter?.Trim() ?? "",
            RoleSearchValue = roleSearchValue?.Trim() ?? "",
            RoleFilter = roleFilter?.Trim() ?? "",
            EditAccountId = editAccountId,
            AccountForm = BuildAccountForm(editAccountId, backofficeAccounts),
            BackofficeAccounts = backofficeAccounts,
            RoleAccounts = roleAccounts,
            PermissionRoles = roles.Select(role => new SystemRoleAdminRowViewModel
            {
                Id = role.Id,
                Name = role.Name,
                DisplayName = role.DisplayName,
                Description = role.Description,
                IsSystem = role.IsSystem,
                UserCount = users.Count(user => user.RoleId == role.Id || user.Role == role.Name)
            }).ToList(),
            PermissionRows = permissions.Select(permission =>
            {
                userOverrides.TryGetValue(permission.Id, out var userOverride);
                var expired = userOverride?.ExpiresAt.HasValue == true && userOverride.ExpiresAt <= DateTime.Now;
                return new SystemPermissionRowViewModel
                {
                    Id = permission.Id,
                    Code = permission.Code,
                    DisplayName = permission.DisplayName,
                    Description = permission.Description,
                    Module = permission.Module,
                    IsSelected = rolePermissionIds.Contains(permission.Id),
                    UserMode = userOverride == null || expired ? "Inherit" : userOverride.IsGranted ? "Grant" : "Deny",
                    ExpiresAt = expired ? null : userOverride?.ExpiresAt,
                    IsApplicationPermission = typeof(PermissionConstants).GetFields()
                        .Any(field => field.IsLiteral && field.FieldType == typeof(string) &&
                            string.Equals((string?)field.GetRawConstantValue(), permission.Code, StringComparison.OrdinalIgnoreCase))
                };
            }).ToList(),
            PermissionUsers = users.Select(user => new SystemUserOptionViewModel
                {
                Id = user.Id,
                Label = $"{user.CustomerName} — {user.Phone}",
                Role = user.Role
            }).ToList(),
            RoleOptions = roles
                .Where(role => !string.Equals(role.Name, CustomerRoles.Customer, StringComparison.OrdinalIgnoreCase))
                .Select(role => new SystemRoleOptionViewModel { Name = role.Name, DisplayName = role.DisplayName })
                .ToList(),
            SelectedPermissionRoleId = selectedRoleId,
            SelectedPermissionUserId = selectedUserId,
            RoleForm = new SystemRoleFormViewModel(),
            PermissionForm = new SystemPermissionFormViewModel(),
            CurrentUserId = currentUserId,
            CanManagePermissions = User.HasAnyPermission(PermissionConstants.ManagePermission, PermissionConstants.ManageSystem),
            CanViewManagement = User.HasAnyPermission(PermissionConstants.ViewReport, PermissionConstants.ViewRevenue, PermissionConstants.ManageSystem),
            CanCreateUsers = User.HasAnyPermission(PermissionConstants.CreateUser, PermissionConstants.ManageSystem),
            CanEditUsers = User.HasAnyPermission(PermissionConstants.EditUser, PermissionConstants.ManageSystem),
            CanResetPasswords = User.HasAnyPermission(PermissionConstants.ResetPassword, PermissionConstants.ManageSystem),
            AdminRole = CustomerRoles.Admin,
            StaffRole = CustomerRoles.Staff,
            AdminRoleLabel = "Quản trị viên",
            StaffRoleLabel = "Nhân viên"
        };
    }

    private SystemAccountFormViewModel BuildAccountForm(
        int? editAccountId,
        List<SystemAccountRowViewModel> accounts)
    {
        if (!editAccountId.HasValue)
            return new SystemAccountFormViewModel { Role = CustomerRoles.Staff };

        var account = accounts.FirstOrDefault(a => a.Id == editAccountId.Value);
        if (account == null)
            return new SystemAccountFormViewModel { Role = CustomerRoles.Staff };

        return new SystemAccountFormViewModel
        {
            Id = account.Id,
            CustomerName = account.CustomerName,
            Phone = account.Phone,
            Email = account.Email,
            Address = account.Address,
            Role = account.Role
        };
    }

    private static SystemAccountRowViewModel ToRow(SystemAccountData data, int currentUserId) => new()
    {
        Id = data.Id,
        CustomerName = data.CustomerName,
        Phone = data.Phone,
        Email = data.Email,
        Address = data.Address,
        Role = data.Role,
        CreatedTime = data.CreatedTime,
        IsCurrentUser = data.Id == currentUserId
    };

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst("CustomerId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null && int.TryParse(claim.Value, out var id) ? id : 0;
    }
}
