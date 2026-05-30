
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class AccountService : IAccountService
    {
        private readonly ICustomerDAL _customerDAL;
        private readonly IPasswordHasherService _passwordHasherService;

        public AccountService(ICustomerDAL customerDAL, IPasswordHasherService passwordHasherService)
        {
            _customerDAL = customerDAL;
            _passwordHasherService = passwordHasherService;
        }

        public Customer? GetCustomerById(int id)
        {
            return _customerDAL.GetById(id);
        }

        public Customer? Login(string phone, string password)
        {
            var customer = _customerDAL.GetByPhone(phone.Trim());
            if (customer == null || string.IsNullOrWhiteSpace(customer.Password))
                return null;

            return _passwordHasherService.Verify(password, customer.Password) ? customer : null;
        }

        public bool Register(Customer data)
        {
            var phone = data.Phone?.Trim() ?? "";
            if (_customerDAL.GetByPhone(phone) != null)
                return false;

            data.Role = string.IsNullOrWhiteSpace(data.Role) ? "Customer" : data.Role.Trim();
            data.CustomerName = data.CustomerName?.Trim() ?? "";
            data.Phone = phone;
            data.Password = data.Password?.Trim() ?? "";
            data.Address = data.Address?.Trim();
            data.Email = data.Email?.Trim();

            if (string.IsNullOrWhiteSpace(data.CustomerName) ||
                string.IsNullOrWhiteSpace(data.Phone) ||
                string.IsNullOrWhiteSpace(data.Password))
            {
                return false;
            }

            data.Password = _passwordHasherService.Hash(data.Password);
            return _customerDAL.Add(data) > 0;
        }

        public bool UpdateProfile(int id, string customerName, string? email, string? address)
        {
            var existing = _customerDAL.GetById(id);
            if (existing == null)
                return false;

            var normalizedName = customerName?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(normalizedName))
                return false;

            existing.CustomerName = normalizedName;
            existing.Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
            existing.Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();

            return _customerDAL.UpdateProfile(existing);
        }

        public string GenerateOTP()
        {
            return Random.Shared.Next(100000, 999999).ToString();
        }
    }
}
