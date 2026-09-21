using SV22T1080045.Shop.Abstractions.Models.System;

namespace SV22T1080045.Shop.BusinessLayers.Interfaces;

public interface ISystemService
{
    List<SystemAccountData> ListBackofficeAccounts(string? searchValue, string? roleFilter);
    List<SystemAccountData> ListAccountsForRoleAssignment(string? searchValue, string? roleFilter);
    SystemAccountSaveResult SaveBackofficeAccount(SystemAccountInput input, int actorId);
    SystemAccountSaveResult ResetPassword(int accountId, string newPassword, int actorId);
    SystemAccountSaveResult SetAccountActive(int accountId, bool isActive, int actorId);
    SystemRoleUpdateResult UpdateAccountRole(int accountId, string role, int actorId);
}

public class SystemAccountInput
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string Role { get; set; } = "";
    public string? Password { get; set; }
}
