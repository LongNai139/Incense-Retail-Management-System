using Microsoft.EntityFrameworkCore;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class PhoneOtpDAL : IPhoneOtpDAL
    {
        private readonly ShopDbContext _context;

        public PhoneOtpDAL(ShopDbContext context)
        {
            _context = context;
        }

        public string Generate(string phone, string purpose)
        {
            var existingOtps = _context.PhoneOtps
                .Where(o => o.Phone == phone && o.Purpose == purpose)
                .ToList();

            _context.PhoneOtps.RemoveRange(existingOtps);

            var code = Random.Shared.Next(100000, 999999).ToString();

            var otp = new PhoneOtp
            {
                Phone = phone,
                OtpCode = code,
                Purpose = purpose,
                ExpiresAt = DateTime.Now.AddMinutes(5),
                IsUsed = false,
                FailCount = 0,
                CreatedAt = DateTime.Now,
                CreatedTime = DateTime.Now,
                IsDeleted = false
            };

            _context.PhoneOtps.Add(otp);
            _context.SaveChanges();

            return code;
        }

        public bool Verify(string phone, string purpose, string code)
        {
            var otp = _context.PhoneOtps
                .Where(o => o.Phone == phone && o.Purpose == purpose && o.IsUsed == false)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault();

            if (otp == null) return false;
            if (!otp.IsValid()) return false;

            if (otp.OtpCode != code)
            {
                otp.FailCount++;
                _context.SaveChanges();
                return false;
            }

            otp.IsUsed = true;
            _context.SaveChanges();
            return true;
        }

        public void CleanupExpired()
        {
            var expired = _context.PhoneOtps
                .Where(o => o.ExpiresAt < DateTime.Now || o.IsUsed == true)
                .ToList();

            _context.PhoneOtps.RemoveRange(expired);
            _context.SaveChanges();
        }
    }
}
