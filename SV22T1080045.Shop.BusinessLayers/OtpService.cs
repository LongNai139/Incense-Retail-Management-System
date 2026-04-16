using SV22T1080045.Shop.DataLayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.BusinessLayers
{
    // ── Interface ─────────────────────────────────────────────────────────────
    /// <summary>
    /// Tạo OTP, gửi về SĐT và xác thực.
    /// Dùng được cho: đăng nhập, đăng ký, quên mật khẩu, tra cứu đơn hàng.
    /// Không biết đơn hàng hay khách hàng là gì.
    /// </summary>
    public interface IOtpService
    {
        bool Send(string phone, string purpose);
        bool Verify(string phone, string purpose, string code);
    }

    // ── Các mục đích dùng OTP ─────────────────────────────────────────────────
    public static class OtpPurpose
    {
        public const string Login = "login";
        public const string Register = "register";
        public const string OrderLookup = "order_lookup";
        public const string ForgotPw = "forgot_password";
    }

    // ── Implementation ────────────────────────────────────────────────────────
    public class OtpService : IOtpService
    {
        private readonly IPhoneOtpDAL _otpDal;
        private readonly ISmsService _sms;

        public OtpService(IPhoneOtpDAL otpDal, ISmsService sms)
        {
            _otpDal = otpDal;
            _sms = sms;
        }

        public bool Send(string phone, string purpose)
        {
            // 1. Tạo mã OTP mới trong database
            var code = _otpDal.Generate(phone, purpose);

            // 2. Soạn nội dung SMS theo mục đích
            var message = purpose switch
            {
                OtpPurpose.Login => $"[Trầm Hương Shop] Mã đăng nhập: {code}. Hết hạn sau 5 phút.",
                OtpPurpose.Register => $"[Trầm Hương Shop] Mã đăng ký: {code}. Hết hạn sau 5 phút.",
                OtpPurpose.OrderLookup => $"[Trầm Hương Shop] Mã tra cứu đơn hàng: {code}. Hết hạn sau 5 phút.",
                OtpPurpose.ForgotPw => $"[Trầm Hương Shop] Mã đặt lại mật khẩu: {code}. Hết hạn sau 5 phút.",
                _ => $"[Trầm Hương Shop] Mã xác thực: {code}. Hết hạn sau 5 phút."
            };

            // 3. Gửi qua SMS (FakeSms hoặc ESMS tuỳ môi trường)
            return _sms.Send(phone, message);
        }

        public bool Verify(string phone, string purpose, string code)
            => _otpDal.Verify(phone, purpose, code);
    }
}
