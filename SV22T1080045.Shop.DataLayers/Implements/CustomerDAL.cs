using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class CustomerDAL : ICustomerDAL
    {
        private readonly ShopDbContext _context;

        public CustomerDAL(ShopDbContext context)
        {
            _context = context;
        }

        public Customer? GetById(int id)
        {
            return _context.Customers
                .FirstOrDefault(c => !c.IsDeleted && c.Id == id);
        }

        public Customer? GetByPhone(string phone)
        {
            var normalizedPhone = phone.Trim();
            return _context.Customers
                .FirstOrDefault(c => !c.IsDeleted && c.Phone == normalizedPhone);
        }

        public Customer? Authenticate(string phone, string password)
        {
            var normalizedPhone = phone.Trim();
            return _context.Customers
                .FirstOrDefault(c => !c.IsDeleted && c.Phone == normalizedPhone && c.Password == password);
        }

        public int Add(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
            return customer.Id;
        }

        public bool UpdateProfile(Customer customer)
        {
            var existing = GetById(customer.Id);
            if (existing == null)
                return false;

            existing.CustomerName = customer.CustomerName;
            existing.Email = customer.Email;
            existing.Address = customer.Address;

            _context.SaveChanges();
            return true;
        }
    }
}
