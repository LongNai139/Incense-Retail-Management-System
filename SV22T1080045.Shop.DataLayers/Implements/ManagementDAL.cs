using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.Abstractions.Models.Management;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class ManagementDAL : IManagementDAL
    {
        private readonly ShopDbContext _context;

        public ManagementDAL(ShopDbContext context)
        {
            _context = context;
        }

        public bool VoucherCodeExists(string code, int exceptId)
        {
            var normalizedCode = code.Trim().ToUpperInvariant();
            return _context.Vouchers
                .Any(v => !v.IsDeleted && v.Code == normalizedCode && v.Id != exceptId);
        }

        public int AddVoucher(Voucher voucher)
        {
            voucher.Code = voucher.Code.Trim().ToUpperInvariant();
            voucher.CreatedTime = DateTime.Now;
            voucher.IsDeleted = false;

            _context.Vouchers.Add(voucher);
            _context.SaveChanges();
            return voucher.Id;
        }

        public Voucher? GetVoucher(int id)
        {
            return _context.Vouchers
                .FirstOrDefault(v => v.Id == id && !v.IsDeleted);
        }

        public bool SetVoucherActive(int id, bool isActive)
        {
            var voucher = _context.Vouchers
                .FirstOrDefault(v => v.Id == id && !v.IsDeleted);
            if (voucher == null)
                return false;

            voucher.IsActive = isActive;
            return _context.SaveChanges() > 0;
        }

        public bool SetVoucherDeleted(int id, bool isDeleted)
        {
            var voucher = _context.Vouchers
                .FirstOrDefault(v => v.Id == id);
            if (voucher == null)
                return false;

            voucher.IsDeleted = isDeleted;
            return _context.SaveChanges() > 0;
        }

        public List<Voucher> ListVouchers(int take)
        {
            return _context.Vouchers
                .Where(v => !v.IsDeleted)
                .OrderByDescending(v => v.CreatedTime)
                .Take(take)
                .ToList();
        }

        public List<ManagementOrderData> ListRecentOrders(int take)
        {
            var query = from o in _context.Orders
                        join c in _context.Customers on o.CustomerId equals c.Id into customerGroup
                        from c in customerGroup.DefaultIfEmpty()
                        join d in _context.OrderDetails on o.Id equals d.OrderId into detailGroup
                        from d in detailGroup.DefaultIfEmpty()
                        where !o.IsDeleted
                        group new { o, c, d } by new
                        {
                            o.Id,
                            o.OrderDate,
                            o.TotalAmount,
                            o.Status,
                            CustomerName = c != null && !string.IsNullOrWhiteSpace(c.CustomerName)
                                ? c.CustomerName
                                : (!string.IsNullOrWhiteSpace(o.ShippingName) ? o.ShippingName : "Guest"),
                            CustomerPhone = c != null && !string.IsNullOrWhiteSpace(c.Phone)
                                ? c.Phone
                                : (!string.IsNullOrWhiteSpace(o.ShippingPhone) ? o.ShippingPhone : ""),
                            IsGuest = c == null || o.CustomerId <= 0
                        } into g
                        select new ManagementOrderData
                        {
                            Id = g.Key.Id,
                            OrderDate = g.Key.OrderDate,
                            TotalAmount = g.Key.TotalAmount,
                            Status = g.Key.Status,
                            CustomerName = g.Key.CustomerName,
                            CustomerPhone = g.Key.CustomerPhone,
                            IsGuest = g.Key.IsGuest,
                            ItemCount = g.Sum(x => x.d != null ? x.d.Quantity : 0),
                            ProductTypeCount = g.Count(x => x.d != null && x.d.ProductId > 0)
                        };

            return query
                .OrderByDescending(o => o.OrderDate)
                .Take(take)
                .ToList();
        }

        public (List<ManagementOrderData> Orders, int TotalCount) ListOrdersPaginated(int page, int pageSize, int? status = null)
        {
            var baseQuery = from o in _context.Orders
                            join c in _context.Customers on o.CustomerId equals c.Id into customerGroup
                            from c in customerGroup.DefaultIfEmpty()
                            join d in _context.OrderDetails on o.Id equals d.OrderId into detailGroup
                            from d in detailGroup.DefaultIfEmpty()
                            where !o.IsDeleted
                            group new { o, c, d } by new
                            {
                                o.Id,
                                o.OrderDate,
                                o.TotalAmount,
                                o.Status,
                                CustomerName = c != null && !string.IsNullOrWhiteSpace(c.CustomerName)
                                    ? c.CustomerName
                                    : (!string.IsNullOrWhiteSpace(o.ShippingName) ? o.ShippingName : "Guest"),
                                CustomerPhone = c != null && !string.IsNullOrWhiteSpace(c.Phone)
                                    ? c.Phone
                                    : (!string.IsNullOrWhiteSpace(o.ShippingPhone) ? o.ShippingPhone : ""),
                                IsGuest = c == null || o.CustomerId <= 0
                            } into g
                            select new
                            {
                                OrderData = new ManagementOrderData
                                {
                                    Id = g.Key.Id,
                                    OrderDate = g.Key.OrderDate,
                                    TotalAmount = g.Key.TotalAmount,
                                    Status = g.Key.Status,
                                    CustomerName = g.Key.CustomerName,
                                    CustomerPhone = g.Key.CustomerPhone,
                                    IsGuest = g.Key.IsGuest,
                                    ItemCount = g.Sum(x => x.d != null ? x.d.Quantity : 0),
                                    ProductTypeCount = g.Count(x => x.d != null && x.d.ProductId > 0)
                                },
                                g.Key.Status
                            };

            if (status.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.Status == status.Value);
            }

            var totalCount = baseQuery.Count();

            var orders = baseQuery
                .OrderByDescending(x => x.OrderData.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.OrderData)
                .ToList();

            return (orders, totalCount);
        }

        public List<CustomerManagementData> ListCustomers()
        {
            return (from c in _context.Customers
                    join o in _context.Orders on c.Id equals o.CustomerId into orderGroup
                    from o in orderGroup.DefaultIfEmpty()
                    where !c.IsDeleted
                    group o by new
                    {
                        c.Id,
                        c.CustomerName,
                        c.Phone,
                        c.Email,
                        c.Address,
                        c.Role,
                        c.CreatedTime
                    } into g
                    select new CustomerManagementData
                    {
                        Id = g.Key.Id,
                        CustomerName = g.Key.CustomerName,
                        Phone = g.Key.Phone,
                        Email = g.Key.Email,
                        Address = g.Key.Address,
                        Role = g.Key.Role,
                        CreatedTime = g.Key.CreatedTime,
                        OrderCount = g.Count(x => x != null && !x.IsDeleted),
                        TotalSpent = g.Where(x => x != null && !x.IsDeleted).Sum(x => x.TotalAmount),
                        LastOrderDate = g.Where(x => x != null && !x.IsDeleted).Max(x => (DateTime?)x.OrderDate)
                    })
                    .OrderByDescending(c => c.LastOrderDate ?? c.CreatedTime)
                    .ToList();
        }

        public List<CustomerPurchaseHistoryData> ListCustomerHistory(int customerId, int take)
        {
            var histories = _context.Orders
                .Where(o => !o.IsDeleted && o.CustomerId == customerId)
                .GroupJoin(
                    _context.OrderDetails,
                    o => o.Id,
                    d => d.OrderId,
                    (o, details) => new { o, details })
                .SelectMany(
                    x => x.details.DefaultIfEmpty(),
                    (o, d) => new { o.o, d })
                .GroupBy(x => new { x.o.Id, x.o.OrderDate, x.o.TotalAmount, x.o.Status })
                .Select(g => new CustomerPurchaseHistoryData
                {
                    OrderId = g.Key.Id,
                    OrderDate = g.Key.OrderDate,
                    TotalAmount = g.Key.TotalAmount,
                    Status = g.Key.Status,
                    ItemCount = g.Sum(x => x.d != null ? x.d.Quantity : 0)
                })
                .OrderByDescending(h => h.OrderDate)
                .Take(take)
                .ToList();

            var orderIds = histories.Select(h => h.OrderId).ToArray();
            if (orderIds.Length == 0)
                return histories;

            var products = _context.OrderDetails
                .Join(_context.Products, d => d.ProductId, p => p.Id, (d, p) => new { d.OrderId, p.ProductName })
                .Where(x => orderIds.Contains(x.OrderId))
                .ToList();

            foreach (var history in histories)
            {
                history.ProductNames = products
                    .Where(p => p.OrderId == history.OrderId)
                    .Select(p => p.ProductName)
                    .Take(3)
                    .ToList();
            }

            return histories;
        }

        public ManagementOrderDetailsData? GetOrderDetails(int orderId)
        {
            var order = (from o in _context.Orders
                         join c in _context.Customers on o.CustomerId equals c.Id into customerGroup
                         from c in customerGroup.DefaultIfEmpty()
                         where !o.IsDeleted && o.Id == orderId
                         select new ManagementOrderDetailHeaderData
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
                             CustomerEmail = c != null ? c.Email : null,
                             ShippingAddress = o.ShippingAddress,
                             IsGuest = c == null || o.CustomerId <= 0
                         })
                        .FirstOrDefault();

            if (order == null)
                return null;

            // LEFT JOIN Products: sản phẩm đã bị xóa vẫn giữ lại dòng trong chi tiết đơn
            var details = (from d in _context.OrderDetails
                           join p in _context.Products on d.ProductId equals p.Id into productGroup
                           from p in productGroup.DefaultIfEmpty()
                           where d.OrderId == orderId
                           select new ManagementOrderDetailLineData
                           {
                               ProductName = p != null ? p.ProductName : "Sản phẩm không còn tồn tại",
                               Quantity = d.Quantity,
                               UnitPrice = d.UnitPrice,
                               LineTotal = d.Quantity * d.UnitPrice
                           })
                          .ToList();

            return new ManagementOrderDetailsData
            {
                Order = order,
                Details = details
            };
        }
    }
}
