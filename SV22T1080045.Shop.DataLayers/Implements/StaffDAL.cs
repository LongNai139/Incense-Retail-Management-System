using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.Abstractions.Models.Staff;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class StaffDAL : IStaffDAL
    {
        private const int RecentProductRowLimit = 300;

        private readonly ShopDbContext _context;

        public StaffDAL(ShopDbContext context)
        {
            _context = context;
        }

        public StaffOrderDashboardData GetOrderDashboardData(DateTime now)
        {
            var today = now.Date;
            var tomorrow = today.AddDays(1);
            var orders = _context.Orders.AsNoTracking()
                .Where(order => !order.IsDeleted && order.OrderDate >= today && order.OrderDate < tomorrow);
            return new StaffOrderDashboardData
            {
                NewOrdersToday = orders.Count(order => order.Status == 1),
                ProcessingOrders = orders.Count(order => order.Status == 1 || order.Status == 2),
                OrdersToDeliver = orders.Count(order => order.Status == 3),
                RecentOrders = GetTodayOrders(8),
                PendingTooLongOrders = GetTodayOrders(5, status: 1, orderBefore: now.AddHours(-2)),
                FailedDeliveryOrders = GetTodayOrders(5, status: -1)
            };
        }

        public List<StaffProductData> GetLowStockProducts(int take, out int totalCount)
        {
            var query = _context.Products.AsNoTracking().Where(p => !p.IsDeleted &&
                (p.Inventory != null ? p.Inventory.Quantity : (p.Quantity ?? 0)) > 0 &&
                (p.Inventory != null ? p.Inventory.Quantity : (p.Quantity ?? 0)) <=
                    (p.Inventory != null ? p.Inventory.LowStockThreshold : 5));
            totalCount = query.Count();
            return query.OrderBy(p => p.Inventory != null ? p.Inventory.Quantity : (p.Quantity ?? 0))
                .ThenBy(p => p.ProductName).Take(Math.Clamp(take, 0, 100))
                .Select(p => new StaffProductData
                {
                    Id = p.Id, ProductName = p.ProductName,
                    CategoryName = p.Category != null ? p.Category.CategoryName : "Chua phan loai",
                    UnitName = p.Unit != null ? p.Unit.UnitName : "",
                    Quantity = p.Inventory != null ? p.Inventory.Quantity : (p.Quantity ?? 0),
                    SoldCount = p.SoldCount ?? 0, PriceAfterDiscount = p.PriceAfterDiscount, Origin = p.Origin
                }).ToList();
        }

        /// <summary>Đơn của NGÀY HÔM NAY (lọc ngay trong SQL), mới nhất trước, kèm danh sách món.</summary>
        public List<StaffOrderData> GetTodayOrders(int take, string? keyword = null, int? status = null, DateTime? orderBefore = null)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var query = from o in _context.Orders
                          join c in _context.Customers on o.CustomerId equals c.Id into customerGroup
                          from c in customerGroup.DefaultIfEmpty()
                          where !o.IsDeleted &&
                              ((o.OrderDate >= today && o.OrderDate < tomorrow) ||
                               _context.CustomerDebts.Any(d => d.OrderId == o.Id && !d.IsDeleted && d.PaidAmount < d.DebtAmount) ||
                               (o.PaymentMethod == 4 && o.PaymentStatus == 4))
                          select new { Order = o, Customer = c };
            if (status.HasValue)
                query = query.Where(item => item.Order.Status == status.Value);
            if (orderBefore.HasValue)
                query = query.Where(item => item.Order.OrderDate <= orderBefore.Value);
            var normalizedKeyword = keyword?.Trim();
            if (!string.IsNullOrWhiteSpace(normalizedKeyword))
            {
                var orderCode = normalizedKeyword.StartsWith("#TH", StringComparison.OrdinalIgnoreCase)
                    ? normalizedKeyword[3..].TrimStart('0') : normalizedKeyword;
                var hasOrderId = int.TryParse(orderCode, out var orderId);
                query = query.Where(item =>
                    (hasOrderId && item.Order.Id == orderId) ||
                    (item.Customer != null && ((item.Customer.CustomerName != null && item.Customer.CustomerName.Contains(normalizedKeyword)) ||
                        (item.Customer.Phone != null && item.Customer.Phone.Contains(normalizedKeyword)))) ||
                    (item.Order.ShippingName != null && item.Order.ShippingName.Contains(normalizedKeyword)) ||
                    (item.Order.ShippingPhone != null && item.Order.ShippingPhone.Contains(normalizedKeyword)));
            }
            var orders = query.OrderByDescending(item => item.Order.OrderDate)
                          .Take(Math.Clamp(take, 1, 500))
                          .Select(item => new StaffOrderData
                          {
                              Id = item.Order.Id,
                              OrderDate = item.Order.OrderDate,
                              TotalAmount = item.Order.TotalAmount,
                              FinalAmount = item.Order.FinalAmount > 0
                                  ? item.Order.FinalAmount
                                  : Math.Max(0, item.Order.TotalAmount - item.Order.DiscountAmount),
                              Status = item.Order.Status,
                              PaymentMethod = item.Order.PaymentMethod,
                              PaymentStatus = item.Order.PaymentStatus,
                              PaymentTransactionNo = item.Order.PaymentTransactionNo,
                              PaidAt = item.Order.PaidAt,
                              PaidAmount = item.Order.PaymentStatus == 1
                                  ? (item.Order.FinalAmount > 0 ? item.Order.FinalAmount : Math.Max(0, item.Order.TotalAmount - item.Order.DiscountAmount))
                                  : 0,
                              RemainingAmount = item.Order.PaymentStatus == 1
                                  ? 0
                                  : (item.Order.FinalAmount > 0 ? item.Order.FinalAmount : Math.Max(0, item.Order.TotalAmount - item.Order.DiscountAmount)),
                              CustomerName = item.Customer != null && !string.IsNullOrWhiteSpace(item.Customer.CustomerName)
                                  ? item.Customer.CustomerName
                                  : (!string.IsNullOrWhiteSpace(item.Order.ShippingName) ? item.Order.ShippingName : "Guest"),
                              CustomerPhone = item.Customer != null && !string.IsNullOrWhiteSpace(item.Customer.Phone)
                                  ? item.Customer.Phone
                                  : (!string.IsNullOrWhiteSpace(item.Order.ShippingPhone) ? item.Order.ShippingPhone : ""),
                              ShippingAddress = item.Order.ShippingAddress,
                              IsGuest = item.Customer == null || item.Order.CustomerId <= 0
                          }).ToList();

            var orderIds = orders.Select(o => o.Id).ToArray();
            if (orderIds.Length > 0)
            {
                var debts = _context.CustomerDebts.AsNoTracking()
                    .Where(d => !d.IsDeleted && d.OrderId.HasValue && orderIds.Contains(d.OrderId.Value))
                    .Select(d => new { d.Id, OrderId = d.OrderId!.Value, d.DebtAmount, d.PaidAmount })
                    .ToList();
                foreach (var order in orders)
                {
                    var debt = debts.FirstOrDefault(d => d.OrderId == order.Id);
                    if (debt == null)
                        continue;
                    order.CustomerDebtId = debt.Id;
                    order.PaidAmount = debt.PaidAmount;
                    order.RemainingAmount = Math.Max(0, debt.DebtAmount - debt.PaidAmount);
                    order.PaymentStatus = order.RemainingAmount <= 0 ? 1 : debt.PaidAmount > 0 ? 3 : 0;
                }
                var debtIds = debts.Select(d => d.Id).ToArray();
                if (debtIds.Length > 0)
                {
                    var history = _context.CustomerDebtTransactions.AsNoTracking()
                        .Where(t => !t.IsDeleted && debtIds.Contains(t.CustomerDebtId))
                        .OrderByDescending(t => t.TransactionDate)
                        .Select(t => new
                        {
                            t.CustomerDebtId,
                            t.TransactionType,
                            t.Amount,
                            t.BalanceAfter,
                            t.Reference,
                            t.Description,
                            t.TransactionDate
                        })
                        .ToList();
                    foreach (var order in orders)
                    {
                        var debt = debts.FirstOrDefault(d => d.OrderId == order.Id);
                        if (debt == null)
                            continue;
                        order.DebtTransactions = history.Where(t => t.CustomerDebtId == debt.Id)
                            .Select(t => new StaffDebtTransactionData
                            {
                                TransactionType = t.TransactionType,
                                Amount = t.Amount,
                                BalanceAfter = t.BalanceAfter,
                                Reference = t.Reference,
                                Description = t.Description,
                                TransactionDate = t.TransactionDate
                            }).ToList();
                    }
                }
            }
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

            return orders;
        }

        /// <summary>
        /// Một trang sản phẩm (tìm kiếm, sắp xếp, phân trang đều chạy trong SQL).
        /// Trả về cả số trang thực tế sau khi kẹp (clamp) về khoảng hợp lệ.
        /// </summary>
        public StaffProductPageData QueryProducts(string? keyword, int page, int pageSize)
        {
            if (pageSize < 1)
                pageSize = 12;

            var query = _context.Products
                .AsNoTracking()
                .Where(p => !p.IsDeleted);

            var kw = keyword?.Trim();
            if (!string.IsNullOrEmpty(kw))
            {
                query = query.Where(p =>
                    p.ProductName.Contains(kw) ||
                    (p.Category != null && p.Category.CategoryName.Contains(kw)) ||
                    (p.Origin != null && p.Origin.Contains(kw)));
            }

            var totalCount = query.Count();
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
            var currentPage = Math.Clamp(page, 1, totalPages);

            var items = query
                .Select(p => new StaffProductData
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    CategoryName = p.Category != null ? p.Category.CategoryName : "Chua phan loai",
                    UnitName = p.Unit != null ? p.Unit.UnitName : "",
                    Quantity = p.Inventory != null ? p.Inventory.Quantity : (p.Quantity ?? 0),
                    SoldCount = p.SoldCount ?? 0,
                    PriceAfterDiscount = p.PriceAfterDiscount,
                    Origin = p.Origin
                })
                .OrderBy(p => p.Quantity <= 0 ? 0 : p.Quantity < 5 ? 1 : 2)
                .ThenBy(p => p.Quantity)
                .ThenBy(p => p.ProductName)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new StaffProductPageData { Items = items, TotalCount = totalCount, Page = currentPage, PageSize = pageSize };
        }

        /// <summary>Khách mua gần đây nhất, kèm tối đa 3 sản phẩm gần đây của mỗi người.</summary>
        public List<StaffCustomerData> GetRecentCustomers(int take)
        {
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
                            .Take(take)
                            .ToList();

            var customerIds = customers.Select(c => c.Id).ToArray();

            // Giới hạn số dòng đọc về (trước đây không giới hạn: khách mua nhiều sẽ kéo cả lịch sử).
            var recentProducts = customerIds.Length == 0
                ? new List<(int CustomerId, string ProductName)>()
                : (from o in _context.Orders
                   join d in _context.OrderDetails on o.Id equals d.OrderId
                   join p in _context.Products on d.ProductId equals p.Id
                   where !o.IsDeleted && customerIds.Contains(o.CustomerId)
                   orderby o.OrderDate descending
                   select new { o.CustomerId, p.ProductName })
                    .Take(RecentProductRowLimit)
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

            return customers;
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

        public (bool Success, List<InventoryCheckResult> InventoryChecks) UpdateOrderStatusWithInventoryCheck(int orderId, int status)
        {
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.Id == orderId
                    && !o.IsDeleted
                    && o.OrderDate >= DateTime.Today
                    && o.OrderDate < DateTime.Today.AddDays(1));

            if (order == null)
                return (false, new List<InventoryCheckResult>());

            // Chỉ cho chuyển tiếp. Nếu cho đi lùi (2 -> 1) rồi tiến lại (1 -> 2) thì kho bị trừ lần nữa.
            if (status <= order.Status)
                return (false, new List<InventoryCheckResult>());

            // Check inventory only when moving from "Chờ xử lý" (1) to any status > 1 (2, 3, 4)
            if (order.Status == 1 && status > 1)
            {
                var inventoryChecks = new List<InventoryCheckResult>();
                var productIds = order.OrderDetails.Select(d => d.ProductId).Distinct().ToList();
                var products = _context.Products
                    .Include(p => p.Inventory)
                    .Where(p => productIds.Contains(p.Id))
                    .ToDictionary(p => p.Id);

                foreach (var detail in order.OrderDetails)
                {
                    var product = products.GetValueOrDefault(detail.ProductId);
                    var availableQuantity = product?.Inventory?.Quantity ?? product?.Quantity ?? 0;

                    inventoryChecks.Add(new InventoryCheckResult
                    {
                        ProductId = detail.ProductId,
                        ProductName = product?.ProductName ?? "Unknown",
                        RequestedQuantity = detail.Quantity,
                        AvailableQuantity = availableQuantity,
                        IsSufficient = availableQuantity >= detail.Quantity
                    });
                }

                // If any product has insufficient inventory, don't update status
                if (inventoryChecks.Any(c => !c.IsSufficient))
                {
                    return (false, inventoryChecks);
                }

                // All checks passed - deduct inventory
                foreach (var detail in order.OrderDetails)
                {
                    var product = products.GetValueOrDefault(detail.ProductId);
                    if (product?.Inventory != null)
                    {
                        product.Inventory.Quantity = Math.Max(0, product.Inventory.Quantity - detail.Quantity);
                        product.Inventory.UpdatedTime = DateTime.Now;
                    }
                    else if (product != null)
                    {
                        product.Quantity = Math.Max(0, (product.Quantity ?? 0) - detail.Quantity);
                    }
                }
            }

            // COD becomes a receivable only after successful delivery. The unique
            // (CustomerId, OrderId) index protects against duplicate completion.
            if (status == 4 && order.PaymentMethod == 1 && order.CustomerId > 0 &&
                !_context.CustomerDebts.Any(d => d.OrderId == order.Id && !d.IsDeleted))
            {
                var debt = new SV22T1080045.Shop.DomainModels.Warehouse.CustomerDebt
                {
                    CustomerId = order.CustomerId,
                    OrderId = order.Id,
                    OrderCode = $"TH{order.Id:D6}",
                    OrderAmount = order.FinalAmount > 0 ? order.FinalAmount : Math.Max(0, order.TotalAmount - order.DiscountAmount),
                    DebtAmount = order.FinalAmount > 0 ? order.FinalAmount : Math.Max(0, order.TotalAmount - order.DiscountAmount),
                    PaidAmount = 0,
                    DueDate = DateTime.Today.AddDays(30),
                    Status = "Pending",
                    Notes = "Phát sinh khi giao thành công đơn COD",
                    CreatedTime = DateTime.Now,
                    IsDeleted = false
                };
                debt.Transactions.Add(new SV22T1080045.Shop.DomainModels.Warehouse.CustomerDebtTransaction
                {
                    OrderId = order.Id,
                    TransactionType = "DEBIT",
                    Amount = order.FinalAmount > 0 ? order.FinalAmount : Math.Max(0, order.TotalAmount - order.DiscountAmount),
                    BalanceAfter = order.FinalAmount > 0 ? order.FinalAmount : Math.Max(0, order.TotalAmount - order.DiscountAmount),
                    Reference = $"ORDER-{order.Id}",
                    Description = "Phát sinh từ đơn hàng COD đã giao thành công",
                    TransactionDate = DateTime.Now,
                    CreatedTime = DateTime.Now,
                    IsDeleted = false
                });
                _context.CustomerDebts.Add(debt);
            }

            order.Status = status;
            try
            {
                var saved = _context.SaveChanges() > 0;
                return (saved, new List<InventoryCheckResult>());
            }
            catch (DbUpdateException exception) when (IsDebtUniqueConstraintViolation(exception))
            {
                // A concurrent completion may have created the debt first. EF's
                // SaveChanges transaction rolls back the order and inventory updates.
                return (false, new List<InventoryCheckResult>());
            }
        }

        private static bool IsDebtUniqueConstraintViolation(DbUpdateException exception)
        {
            return exception.GetBaseException() is SqlException sqlException &&
                   sqlException.Number is 2601 or 2627;
        }
    }
}
