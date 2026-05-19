
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.BusinessLayers.Interfaces;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class OtpService : IOtpService
    {
        private readonly IPhoneOtpDAL _phoneOtpDAL;
        private readonly ISmsService _smsService;

        public OtpService(IPhoneOtpDAL phoneOtpDAL, ISmsService smsService)
        {
            _phoneOtpDAL = phoneOtpDAL;
            _smsService = smsService;
        }

        public bool Send(string phone, string purpose)
        {
            var normalizedPhone = phone.Trim();
            var code = _phoneOtpDAL.Generate(normalizedPhone, purpose);
            var message = purpose switch
            {
                OtpPurpose.Login => $"[Tram Huong Shop] Ma dang nhap: {code}. Het han sau 5 phut.",
                OtpPurpose.Register => $"[Tram Huong Shop] Ma dang ky: {code}. Het han sau 5 phut.",
                OtpPurpose.OrderLookup => $"[Tram Huong Shop] Ma tra cuu don hang: {code}. Het han sau 5 phut.",
                OtpPurpose.ForgotPw => $"[Tram Huong Shop] Ma dat lai mat khau: {code}. Het han sau 5 phut.",
                _ => $"[Tram Huong Shop] Ma xac thuc: {code}. Het han sau 5 phut."
            };

            return _smsService.Send(normalizedPhone, message);
        }

        public bool Verify(string phone, string purpose, string code)
        {
            return _phoneOtpDAL.Verify(phone.Trim(), purpose, code.Trim());
        }
    }
}
