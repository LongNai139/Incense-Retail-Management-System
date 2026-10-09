using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class VoucherDAL : IVoucherDAL
    {
        private readonly ShopDbContext _context;

        public VoucherDAL(ShopDbContext context)
        {
            _context = context;
        }

        public Voucher? GetByCode(string code)
        {
            var normalizedCode = code.Trim().ToUpper();
            return _context.Vouchers
                .FirstOrDefault(v => v.Code == normalizedCode && v.IsActive == true);
        }

        public bool Use(string code)
        {
            var normalizedCode = code.Trim().ToUpper();
            var voucher = _context.Vouchers
                .FirstOrDefault(v => v.Code == normalizedCode);

            if (voucher == null)
                return false;

            voucher.UsedCount++;
            return _context.SaveChanges() > 0;
        }

        public int Add(Voucher v)
        {
            v.Code = v.Code.Trim().ToUpper();
            v.UsedCount = 0;
            v.IsActive = true;
            v.CreatedTime = DateTime.Now;
            v.IsDeleted = false;

            _context.Vouchers.Add(v);
            _context.SaveChanges();
            return v.Id;
        }

        public bool Update(Voucher v)
        {
            v.Code = v.Code.Trim().ToUpper();
            var existing = _context.Vouchers.FirstOrDefault(voucher => voucher.Id == v.Id);
            if (existing == null)
                return false;

            _context.Entry(existing).CurrentValues.SetValues(v);
            return _context.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var voucher = _context.Vouchers.FirstOrDefault(v => v.Id == id);
            if (voucher == null)
                return false;

            voucher.IsActive = false;
            return _context.SaveChanges() > 0;
        }
    }
}
