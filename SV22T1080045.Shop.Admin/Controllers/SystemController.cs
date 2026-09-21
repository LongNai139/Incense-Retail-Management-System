using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1080045.Shop.Abstractions.Models.System;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;
using SV22T1080045.Shop.Models.ViewModels.System;
using System.Security.Claims;

namespace SV22T1080045.Shop.Controllers;

[Authorize(Roles = CustomerRoles.ManageSystem)]
public class SystemController : Controller
{
    private readonly ISystemService _systemService;

    public SystemController(ISystemService systemService)
    {
        _systemService = systemService;
    }

    public IActionResult Index(
        string accountSearchValue = "",
        string accountRoleFilter = "",
        string roleSearchValue = "",
        string roleFilter = "",
        int? editAccountId = null)
    {
        return View(BuildDashboardModel(
            accountSearchValue,
            accountRoleFilter,
            roleSearchValue,
            roleFilter,
            editAccountId));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SaveAccount(SystemAccountFormViewModel form)
    {
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
        return RedirectToAction(nameof(Index), null, new
        {
            editAccountId = result.Success && form.Id <= 0 ? result.AccountId : form.Id > 0 ? form.Id : (int?)null
        }, "accounts");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ResetPassword(int accountId, string newPassword)
    {
        var result = _systemService.ResetPassword(accountId, newPassword, GetCurrentUserId());
        TempData["SystemMessage"] = result.Message;
        return RedirectToAction(nameof(Index), null, new { editAccountId = accountId }, "accounts");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateRole(int accountId, string role)
    {
        var result = _systemService.UpdateAccountRole(accountId, role, GetCurrentUserId());
        TempData["SystemMessage"] = result.Message;
        return RedirectToAction(nameof(Index), null, null, "permissions");
    }

    private SystemDashboardViewModel BuildDashboardModel(
        string accountSearchValue,
        string accountRoleFilter,
        string roleSearchValue,
        string roleFilter,
        int? editAccountId)
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

        var roleCounts = roleAccounts
            .GroupBy(a => a.Role)
            .ToDictionary(g => g.Key, g => g.Count());

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
            RoleDefinitions = BackofficePermissions.Definitions
                .Select(def => new SystemRoleDefinitionViewModel
                {
                    Role = def.Role,
                    DisplayName = def.DisplayName,
                    Description = def.Description,
                    Permissions = def.Permissions.ToList(),
                    AccountCount = roleCounts.TryGetValue(def.Role, out var count) ? count : 0
                })
                .ToList(),
            CurrentUserId = currentUserId
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
