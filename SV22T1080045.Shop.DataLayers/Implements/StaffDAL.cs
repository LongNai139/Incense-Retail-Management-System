using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.Abstractions.Models.Staff;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class StaffDAL : IStaffDAL
    {
        private readonly ShopDbContext _context;

        public StaffDAL(ShopDbContext context)
        {
            _context = context;
        }

        public StaffDashboardData GetDashboardData()
        {
            var orders = (from o in _context.Orders
                          join c in _context.Customers on o.CustomerId equals c.Id into customerGroup
                          from c in customerGroup.DefaultIfEmpty()
                          where !o.IsDeleted
                          orderby o.OrderDate descending
                          select new StaffOrderData
                          {
                              Id = o.Id,
                              OrderDate = o.OrderDate,
                              TotalAmount = o.TotalAmount,
                              Status = o.Status,
                              CustomerName = c != null && !string.IsNullOrWhiteSpace(c.CustomerName)
                                  ? c.CustomerName
                                  : (!string.IsNullOrWhiteSpace(o.ShippingName) ? o.ShippingName : "Guest"),
                              CustomerPhone = c != null && !string.IsNullOrWhiteSpace(c.Phone)
                                  ? c.Phone
                                  : (!string.IsNullOrWhiteSpace(o.ShippingPhone) ? o.ShippingPhone : ""),
                              ShippingAddress = o.ShippingAddress,
                              IsGuest = c == null || o.CustomerId <= 0
                          })
                          .Take(120)
                          .ToList();

            var orderIds = orders.Select(o => o.Id).ToArray();
            var items = orderIds.Length == 0
                ? new List<StaffOrderItemData>()
                : _context.OrderDetails
                    .Include(d => d.Product)
                    .Where(d => orderIds.Contains(d.OrderId))
                    .Select(d => new StaffOrderItemData
                    {
                        OrderId = d.OrderId,
                        ProductName = d.Product != null ? d.Product.ProductName : "",
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice
                    })
                    .ToList();

            foreach (var order in orders)
            {
                order.Items = items.Where(i => i.OrderId == order.Id).ToList();
                order.ItemCount = order.Items.Sum(i => i.Quantity);
            }

            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Unit)
                .Where(p => !p.IsDeleted)
                .Select(p => new StaffProductData
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    CategoryName = p.Category != null ? p.Category.CategoryName : "Chua phan loai",
                    UnitName = p.Unit != null ? p.Unit.UnitName : "",
                    Quantity = p.Quantity ?? 0,
                    SoldCount = p.SoldCount ?? 0,
                    PriceAfterDiscount = p.PriceAfterDiscount,
                    Origin = p.Origin
                })
                .ToList();

            var customers = (from c in _context.Customers
                            join o in _context.Orders on c.Id equals o.CustomerId into orderGroup
                            from o in orderGroup.DefaultIfEmpty()
                            where !c.IsDeleted && c.Role != "Admin" && c.Role != "Staff"
                            group o by new { c.Id, c.CustomerName, c.Phone, c.Email } into g
                            select new StaffCustomerData
                            {
                                Id = g.Key.Id,
                                CustomerName = g.Key.CustomerName,
                                Phone = g.Key.Phone,
                                Email = g.Key.Email,
                                OrderCount = g.Count(x => x != null && !x.IsDeleted),
                                LastOrderDate = g.Where(x => x != null && !x.IsDeleted).Max(x => (DateTime?)x.OrderDate)
                            })
                            .OrderByDescending(c => c.LastOrderDate)
                            .Take(20)
                            .ToList();

            var customerIds = customers.Select(c => c.Id).ToArray();
            var recentProducts = customerIds.Length == 0
                ? new List<(int CustomerId, string ProductName)>()
                : (from o in _context.Orders
                    join d in _context.OrderDetails on o.Id equals d.OrderId
                    join p in _context.Products on d.ProductId equals p.Id
                    where !o.IsDeleted && customerIds.Contains(o.CustomerId)
                    orderby o.OrderDate descending
                    select new { o.CustomerId, p.ProductName })
                    .AsEnumerable()
                    .Select(x => (x.CustomerId, x.ProductName))
                    .ToList();

            foreach (var customer in customers)
            {
                customer.RecentProducts = recentProducts
                    .Where(p => p.CustomerId == customer.Id)
                    .Select(p => p.ProductName)
                    .Distinct()
                    .Take(3)
                    .ToList();
            }

            return new StaffDashboardData
            {
                Orders = orders,
                Products = products,
                Customers = customers
            };
        }

        public bool UpdateOrderStatus(int orderId, int status)
        {
            var order = _context.Orders
                .FirstOrDefault(o => o.Id == orderId
                    && !o.IsDeleted
                    && o.OrderDate >= DateTime.Today
                    && o.OrderDate < DateTime.Today.AddDays(1));

            if (order == null)
                return false;

            order.Status = status;
            return _context.SaveChanges() > 0;
        }
    }
}
