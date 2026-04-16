using SV22T1080045.Shop.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.DataLayers
{
    public interface ICustomerDAL
    {
        // Lấy thông tin khách hàng dựa trên số điện thoại
        Customer? GetByPhone(string phone);

        // Kiểm tra đăng nhập (nếu bạn muốn check cả phone và password ở tầng SQL)
        Customer? Authenticate(string phone, string password);
        int Add(Customer customer);
    }
    public class CustomerDAL : ICustomerDAL
    {
        private readonly ShopDbContext _context;
        public CustomerDAL(ShopDbContext context)
        {
            _context = context;
        }
        public Customer? GetByPhone(string phone)
        {
            return _context.Customers.FirstOrDefault(c => c.Phone == phone);
        }
        public Customer? Authenticate(string phone, string password)
        {
            return _context.Customers.FirstOrDefault(c => c.Phone == phone && c.Password == password);
        }

        public int Add(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
            return customer.Id;
        }
    }
}
