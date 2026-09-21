using SV22T1080045.Shop.Abstractions.Models.System;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.Abstractions.Interfaces;

public interface ISystemDAL
{
    List<SystemAccountData> ListBackofficeAccounts(string? searchValue, string? roleFilter);
    List<SystemAccountData> ListAccountsForRoleAssignment(string? searchValue, string? roleFilter);
    Customer? GetAccountById(int id);
    Customer? GetByPhone(string phone);
    int AddAccount(Customer customer);
    bool UpdateAccount(Customer customer);
    bool UpdateRole(int id, string role);
    bool SetDeleted(int id, bool isDeleted);
    int CountActiveAdmins(int? exceptId = null);
}
