using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.Abstractions.Models.System;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers.Implements;

public class SystemDAL : ISystemDAL
{
    private readonly ShopDbContext _context;

    public SystemDAL(ShopDbContext context)
    {
        _context = context;
    }

    public List<SystemAccountData> ListBackofficeAccounts(string? searchValue, string? roleFilter)
    {
        var query = _context.Customers
            .Where(c => !c.IsDeleted && (c.Role == CustomerRoles.Admin || c.Role == CustomerRoles.Staff));

        query = ApplyFilters(query, searchValue, roleFilter);

        return query
            .OrderByDescending(c => c.CreatedTime)
            .Select(c => ToData(c))
            .ToList();
    }

    public List<SystemAccountData> ListAccountsForRoleAssignment(string? searchValue, string? roleFilter)
    {
        var query = _context.Customers.Where(c => !c.IsDeleted);
        query = ApplyFilters(query, searchValue, roleFilter);

        return query
            .OrderByDescending(c => c.CreatedTime)
            .Select(c => ToData(c))
            .ToList();
    }

    public Customer? GetAccountById(int id)
    {
        return _context.Customers.FirstOrDefault(c => c.Id == id && !c.IsDeleted);
    }

    public Customer? GetByPhone(string phone)
    {
        var normalizedPhone = phone.Trim();
        return _context.Customers.FirstOrDefault(c => !c.IsDeleted && c.Phone == normalizedPhone);
    }

    public int AddAccount(Customer customer)
    {
        _context.Customers.Add(customer);
        _context.SaveChanges();
        return customer.Id;
    }

    public bool UpdateAccount(Customer customer)
    {
        var existing = GetAccountById(customer.Id);
        if (existing == null)
            return false;

        existing.CustomerName = customer.CustomerName;
        existing.Phone = customer.Phone;
        existing.Email = customer.Email;
        existing.Address = customer.Address;
        existing.Role = customer.Role;

        if (!string.IsNullOrWhiteSpace(customer.Password))
            existing.Password = customer.Password;

        _context.SaveChanges();
        return true;
    }

    public bool UpdateRole(int id, string role)
    {
        var existing = GetAccountById(id);
        if (existing == null)
            return false;

        existing.Role = role;
        _context.SaveChanges();
        return true;
    }

    public bool SetDeleted(int id, bool isDeleted)
    {
        var existing = _context.Customers.FirstOrDefault(c => c.Id == id);
        if (existing == null)
            return false;

        existing.IsDeleted = isDeleted;
        _context.SaveChanges();
        return true;
    }

    public int CountActiveAdmins(int? exceptId = null)
    {
        var query = _context.Customers
            .Where(c => !c.IsDeleted && c.Role == CustomerRoles.Admin);

        if (exceptId.HasValue)
            query = query.Where(c => c.Id != exceptId.Value);

        return query.Count();
    }

    private static IQueryable<Customer> ApplyFilters(
        IQueryable<Customer> query,
        string? searchValue,
        string? roleFilter)
    {
        var keyword = searchValue?.Trim();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(c =>
                c.CustomerName.Contains(keyword) ||
                c.Phone.Contains(keyword) ||
                (c.Email != null && c.Email.Contains(keyword)));
        }

        var role = roleFilter?.Trim();
        if (!string.IsNullOrWhiteSpace(role))
            query = query.Where(c => c.Role == role);

        return query;
    }

    private static SystemAccountData ToData(Customer customer) => new()
    {
        Id = customer.Id,
        CustomerName = customer.CustomerName,
        Phone = customer.Phone,
        Email = customer.Email,
        Address = customer.Address,
        Role = customer.Role,
        CreatedTime = customer.CreatedTime,
        IsDeleted = customer.IsDeleted
    };
}
