using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class StartupSeedService : IStartupSeedService
    {
        private readonly ICustomerDAL _customerDAL;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IRoleDAL _roleDAL;

        public StartupSeedService(ICustomerDAL customerDAL, IPasswordHasherService passwordHasherService, IRoleDAL roleDAL)
        {
            _customerDAL = customerDAL;
            _passwordHasherService = passwordHasherService;
            _roleDAL = roleDAL;
        }

        public void SeedStaffAccounts()
        {
            // Lấy RoleId cho Staff
            var staffRole = _roleDAL.GetByName(CustomerRoles.Staff);
            int? staffRoleId = staffRole?.Id;

            var staffAccounts = new[]
            {
                new Customer
                {
                    CustomerName = "Nhan vien ban hang 01",
                    Phone = "0911000001",
                    Password = _passwordHasherService.Hash("123"),
                    Role = CustomerRoles.Staff,
                    RoleId = staffRoleId,
                    Address = "Cua hang Huong Tram",
                    Email = "staff01@tramhuong.local",
                    CreatedTime = DateTime.Now,
                    IsDeleted = false
                },
                new Customer
                {
                    CustomerName = "Nhan vien kho 02",
                    Phone = "0911000002",
                    Password = _passwordHasherService.Hash("123"),
                    Role = CustomerRoles.Staff,
                    RoleId = staffRoleId,
                    Address = "Cua hang Huong Tram",
                    Email = "staff02@tramhuong.local",
                    CreatedTime = DateTime.Now,
                    IsDeleted = false
                }
            };

            foreach (var account in staffAccounts)
            {
                if (_customerDAL.GetByPhone(account.Phone) == null)
                    _customerDAL.Add(account);
            }
        }
    }
}
