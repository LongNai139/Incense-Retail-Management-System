using SV22T1080045.Shop.BusinessLayers.Interfaces;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class FakeSmsService : ISmsService
    {
        public bool Send(string phone, string message)
        {
            Console.WriteLine($"[DEV-SMS] -> {phone}: {message}");
            return true;
        }
    }

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
                    new KeyValuePair<string, string>("ApiKey", _apiKey),
                    new KeyValuePair<string, string>("Content", message),
                    new KeyValuePair<string, string>("Phone", phone),
                    new KeyValuePair<string, string>("SecretKey", _secretKey),
                    new KeyValuePair<string, string>("SmsType", "4"),
                    new KeyValuePair<string, string>("IsUnicode", "0")
                });

                var response = http.PostAsync(
                    "https://rest.esms.vn/MainService.svc/json/SendMultipleMessage_V4_post_json/",
                    payload).Result;

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
