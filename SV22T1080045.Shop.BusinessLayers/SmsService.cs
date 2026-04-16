using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV22T1080045.Shop.BusinessLayers
{
    // ── Interface ─────────────────────────────────────────────────────────────
    /// <summary>
    /// Chỉ làm 1 việc: gửi tin nhắn SMS về một số điện thoại.
    /// Không biết OTP là gì, không biết đơn hàng là gì.
    /// </summary>
    public interface ISmsService
    {
        bool Send(string phone, string message);
    }

    // ── Dev: log ra console, không gửi thật ──────────────────────────────────
    public class FakeSmsService : ISmsService
    {
        public bool Send(string phone, string message)
        {
            Console.WriteLine($"[DEV-SMS] → {phone}: {message}");
            return true;
        }
    }

    // ── Production: gửi qua ESMS.vn ──────────────────────────────────────────
    /// <summary>
    /// Cấu hình trong appsettings.json:
    /// "Sms": { "Provider": "esms", "ApiKey": "...", "SecretKey": "..." }
    /// </summary>
    public class EsmsSmsService : ISmsService
    {
        private readonly string _apiKey;
        private readonly string _secretKey;

        public EsmsSmsService(string apiKey, string secretKey)
        {
            _apiKey = apiKey;
            _secretKey = secretKey;
        }

        public bool Send(string phone, string message)
        {
            try
            {
                using var http = new HttpClient();
                var payload = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string,string>("ApiKey",    _apiKey),
                    new KeyValuePair<string,string>("Content",   message),
                    new KeyValuePair<string,string>("Phone",     phone),
                    new KeyValuePair<string,string>("SecretKey", _secretKey),
                    new KeyValuePair<string,string>("SmsType",   "4"),
                    new KeyValuePair<string,string>("IsUnicode", "0"),
                });
                var res = http.PostAsync(
                    "https://rest.esms.vn/MainService.svc/json/SendMultipleMessage_V4_post_json/",
                    payload).Result;
                return res.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
