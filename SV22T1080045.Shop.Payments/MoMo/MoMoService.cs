using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SV22T1080045.Shop.Payments.MoMo
{
    public class MoMoService : IMoMoService
    {
        private readonly MoMoOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;

        public MoMoService(IOptions<MoMoOptions> options, IHttpClientFactory httpClientFactory)
        {
            _options = options.Value;
            _httpClientFactory = httpClientFactory;
        }

        public bool IsConfigured => _options.IsConfigured;

        public async Task<MoMoPaymentResult> CreatePaymentAsync(
            int orderId,
            decimal amount,
            string orderInfo,
            string redirectUrl,
            string ipnUrl,
            CancellationToken cancellationToken = default)
        {
            if (!IsConfigured)
            {
                return new MoMoPaymentResult
                {
                    IsSuccess = false,
                    Message = "MoMo chưa được cấu hình PartnerCode/AccessKey/SecretKey."
                };
            }

            var requestId = $"{orderId}-{DateTime.UtcNow:yyyyMMddHHmmss}";
            var momoOrderId = $"TH{orderId:D6}";
            var amountLong = (long)Math.Round(amount, 0, MidpointRounding.AwayFromZero);
            var extraData = string.IsNullOrWhiteSpace(_options.ExtraData) ? "" : _options.ExtraData;

            var rawSignature =
                $"accessKey={_options.AccessKey}" +
                $"&amount={amountLong}" +
                $"&extraData={extraData}" +
                $"&ipnUrl={ipnUrl}" +
                $"&orderId={momoOrderId}" +
                $"&orderInfo={orderInfo}" +
                $"&partnerCode={_options.PartnerCode}" +
                $"&redirectUrl={redirectUrl}" +
                $"&requestId={requestId}" +
                $"&requestType={_options.RequestType}";

            var signature = HmacSha256(_options.SecretKey, rawSignature);

            var payload = new
            {
                partnerCode = _options.PartnerCode,
                partnerName = _options.PartnerName,
                storeId = string.IsNullOrWhiteSpace(_options.StoreId) ? _options.PartnerCode : _options.StoreId,
                requestId,
                amount = amountLong,
                orderId = momoOrderId,
                orderInfo,
                redirectUrl,
                ipnUrl,
                lang = _options.Lang,
                requestType = _options.RequestType,
                autoCapture = true,
                extraData,
                signature
            };

            var client = _httpClientFactory.CreateClient(nameof(MoMoService));
            using var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            using var response = await client.PostAsync(_options.Endpoint, content, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new MoMoPaymentResult
                {
                    IsSuccess = false,
                    Message = $"MoMo API lỗi HTTP {(int)response.StatusCode}: {body}"
                };
            }

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            var resultCode = root.TryGetProperty("resultCode", out var rc) ? rc.GetInt32() : -1;
            var message = root.TryGetProperty("message", out var msg) ? msg.GetString() ?? "" : "";
            var payUrl = root.TryGetProperty("payUrl", out var url) ? url.GetString() : null;

            return new MoMoPaymentResult
            {
                IsSuccess = resultCode == 0 && !string.IsNullOrWhiteSpace(payUrl),
                PayUrl = payUrl,
                ResultCode = resultCode,
                Message = resultCode == 0 ? "Tạo link MoMo thành công." : $"MoMo từ chối: {message} (mã {resultCode})"
            };
        }

        public MoMoIpnResult ReadIpn(IQueryCollection query) => ParseCallback(query);

        public MoMoIpnResult ReadReturn(IQueryCollection query) => ParseCallback(query);

        private MoMoIpnResult ParseCallback(IQueryCollection query)
        {
            var map = query.ToDictionary(
                p => p.Key,
                p => p.Value.ToString(),
                StringComparer.OrdinalIgnoreCase);

            var orderIdRaw = Get(map, "orderId");
            int? orderId = null;
            if (!string.IsNullOrWhiteSpace(orderIdRaw) && orderIdRaw.StartsWith("TH", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(orderIdRaw[2..], NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
                    orderId = parsed;
            }

            var resultCode = int.TryParse(Get(map, "resultCode"), out var rc) ? rc : -1;
            var isValid = VerifyCallbackSignature(map);
            var isSuccess = isValid && resultCode == 0;

            long transId = long.TryParse(Get(map, "transId"), out var tid) ? tid : 0;

            return new MoMoIpnResult
            {
                IsValidSignature = isValid,
                IsSuccess = isSuccess,
                OrderId = orderId,
                TransId = transId,
                RequestId = Get(map, "requestId"),
                Message = isSuccess ? "Thanh toán MoMo thành công." : $"Thanh toán MoMo chưa thành công (mã {resultCode})."
            };
        }

        private bool VerifyCallbackSignature(IReadOnlyDictionary<string, string> map)
        {
            if (!IsConfigured)
                return false;

            var signature = Get(map, "signature");
            if (string.IsNullOrWhiteSpace(signature))
                return false;

            var raw =
                $"accessKey={_options.AccessKey}" +
                $"&amount={Get(map, "amount")}" +
                $"&extraData={Get(map, "extraData")}" +
                $"&message={Get(map, "message")}" +
                $"&orderId={Get(map, "orderId")}" +
                $"&orderInfo={Get(map, "orderInfo")}" +
                $"&orderType={Get(map, "orderType")}" +
                $"&partnerCode={Get(map, "partnerCode")}" +
                $"&payType={Get(map, "payType")}" +
                $"&requestId={Get(map, "requestId")}" +
                $"&responseTime={Get(map, "responseTime")}" +
                $"&resultCode={Get(map, "resultCode")}" +
                $"&transId={Get(map, "transId")}";

            var expected = HmacSha256(_options.SecretKey, raw);
            return string.Equals(expected, signature, StringComparison.OrdinalIgnoreCase);
        }

        private static string Get(IReadOnlyDictionary<string, string> map, string key)
            => map.TryGetValue(key, out var value) ? value : "";

        private static string HmacSha256(string key, string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data))).ToLowerInvariant();
        }
    }
}
