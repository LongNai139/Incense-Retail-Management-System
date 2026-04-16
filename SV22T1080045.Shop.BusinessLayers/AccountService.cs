using SV22T1080045.Shop.DataLayers;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers
{
    public class AccountService
    {
        private readonly ICustomerDAL _customerDAL;
        private readonly IPasswordHasherService _passwordHasherService;

        public AccountService(ICustomerDAL customerDAL, IPasswordHasherService passwordHasherService)
        {
            _customerDAL = customerDAL;
            _passwordHasherService = passwordHasherService;
        }

        // ĐĂNG NHẬP
        public Customer? Login(string phone, string password)
        {
            var customer = _customerDAL.GetByPhone(phone);
            if (customer == null || string.IsNullOrWhiteSpace(customer.Password))
                return null;

            return _passwordHasherService.Verify(password, customer.Password) ? customer : null;
        }

        // ĐĂNG KÝ (Dành cho tab Register của bạn)
        public bool Register(Customer data)
        {
            // Kiểm tra số điện thoại đã tồn tại chưa
            if (_customerDAL.GetByPhone(data.Phone) != null)
                return false;

            data.Role = string.IsNullOrWhiteSpace(data.Role) ? "Customer" : data.Role;
            data.CustomerName = data.CustomerName?.Trim() ?? "";
            data.Phone = data.Phone?.Trim() ?? "";
            data.Password = data.Password?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(data.CustomerName) ||
                string.IsNullOrWhiteSpace(data.Phone) ||
                string.IsNullOrWhiteSpace(data.Password))
            {
                return false;
            }

            data.Password = _passwordHasherService.Hash(data.Password);
            return _customerDAL.Add(data) > 0;
        }

        // LOGIC OTP (Giả lập để khớp với giao diện của bạn)
        public string GenerateOTP()
        {
            // Tạo mã 6 số ngẫu nhiên
            Random res = new Random();
            return res.Next(100000, 999999).ToString();
        }
    }
}