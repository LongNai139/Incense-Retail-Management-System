using Microsoft.EntityFrameworkCore;
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
                .AsNoTracking()
                .Include(c => c.RoleEntity)
                .Include(c => c.Group)
                .FirstOrDefault(c => !c.IsDeleted && c.Id == id);
        }

        public Customer? GetByPhone(string phone)
        {
            var normalizedPhone = phone.Trim();
            return _context.Customers
                .AsNoTracking()
                .Include(c => c.RoleEntity)
                .Include(c => c.Group)
                .FirstOrDefault(c => !c.IsDeleted && c.Phone == normalizedPhone);
        }

        public Customer? Authenticate(string phone, string password)
        {
            var normalizedPhone = phone.Trim();
            return _context.Customers
                .AsNoTracking()
                .Include(c => c.RoleEntity)
                .Include(c => c.Group)
                .FirstOrDefault(c => !c.IsDeleted && c.Phone == normalizedPhone && c.Password == password);
        }

        public List<Customer> ListForPermissionAssignment()
        {
            return _context.Customers
                .AsNoTracking()
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.CustomerName)
                .Select(c => new Customer
                {
                    Id = c.Id,
                    CustomerName = c.CustomerName,
                    Phone = c.Phone,
                    Role = c.Role,
                    RoleId = c.RoleId
                })
                .ToList();
        }

        public int Add(Customer customer)
        {
            // Tự động gán RoleId nếu Role được set nhưng RoleId chưa có
            if (!string.IsNullOrWhiteSpace(customer.Role) && customer.RoleId == null)
            {
                customer.RoleId = _context.Roles
                    .Where(r => r.Name == customer.Role)
                    .Select(r => (int?)r.Id)
                    .FirstOrDefault();
            }

            _context.Customers.Add(customer);
            _context.SaveChanges();
            return customer.Id;
        }

        public bool UpdateProfile(Customer customer)
        {
            var existing = _context.Customers
                .FirstOrDefault(c => !c.IsDeleted && c.Id == customer.Id);
            if (existing == null)
                return false;

            existing.CustomerName = customer.CustomerName;
            existing.Email = customer.Email;
            existing.Address = customer.Address;
            existing.RoleId = customer.RoleId;
            existing.GroupId = customer.GroupId;

            _context.SaveChanges();
            return true;
        }
    }
}
