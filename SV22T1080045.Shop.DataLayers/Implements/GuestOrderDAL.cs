using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.Abstractions;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.DomainModels;
using System.Security.Cryptography;
using System.Text;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class GuestOrderDAL : IGuestOrderDAL
    {
        private readonly ShopDbContext _context;

        public GuestOrderDAL(ShopDbContext context)
        {
            _context = context;
        }

        // ── HASH SĐT (SHA-256, không lưu SĐT gốc) ────────────────────────────
        public static string HashPhone(string phone)
        {
            var normalized = PhoneNumberHelper.NormalizeVietnameseMobile(phone);
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(normalized));
            return Convert.ToHexString(bytes).ToLower();
        }

        private static string LastFour(string phone)
        {
            var digits = PhoneNumberHelper.NormalizeVietnameseMobile(phone);
            return digits.Length >= 4 ? digits[^4..] : digits;
        }

        public void Save(int orderId, string phone)
        {
            var exists = _context.GuestOrders
                .Any(g => g.OrderId == orderId && !g.IsDeleted);
            if (exists) return;

            var guestOrder = new GuestOrder
            {
                OrderId = orderId,
                PhoneHash = HashPhone(phone),
                PhoneLastFour = LastFour(phone),
                CreatedAt = DateTime.Now,
                CreatedTime = DateTime.Now,
                IsDeleted = false
            };

            _context.GuestOrders.Add(guestOrder);
            _context.SaveChanges();
        }

        public List<Order> GetOrdersByPhone(string phone)
        {
            var hash = HashPhone(phone);

            return _context.Orders
                .Join(_context.GuestOrders, o => o.Id, g => g.OrderId, (o, g) => new { o, g })
                .Where(x => x.g.PhoneHash == hash && !x.g.IsDeleted && !x.o.IsDeleted)
                .OrderByDescending(x => x.o.OrderDate)
                .Select(x => x.o)
                .ToList();
        }

        public bool MergeToCustomer(string phone, int customerId)
        {
            var hash = HashPhone(phone);

            var orders = _context.Orders
                .Join(_context.GuestOrders, o => o.Id, g => g.OrderId, (o, g) => new { o, g })
                .Where(x => x.g.PhoneHash == hash && !x.g.IsDeleted && x.o.CustomerId == 0)
                .Select(x => x.o)
                .ToList();

            foreach (var order in orders)
            {
                order.CustomerId = customerId;
            }

            var guestOrders = _context.GuestOrders
                .Where(g => g.PhoneHash == hash && g.ConvertedCustomerId == null)
                .ToList();

            foreach (var guestOrder in guestOrders)
            {
                guestOrder.ConvertedCustomerId = customerId;
            }

            return _context.SaveChanges() > 0;
        }
    }
}
