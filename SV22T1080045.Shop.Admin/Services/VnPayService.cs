using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace SV22T1080045.Shop.Admin.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly VnPayOptions _options;

        public VnPayService(IOptions<VnPayOptions> options)
        {
            _options = options.Value;
        }

        public bool IsConfigured => _options.IsConfigured;

        public string CreatePaymentUrl(int orderId, decimal amount, string orderInfo, string ipAddress, string returnUrl)
        {
            if (!IsConfigured)
                throw new InvalidOperationException("VNPay chưa được cấu hình TmnCode/HashSecret.");

            var now = DateTime.Now;
            var parameters = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                ["vnp_Version"] = _options.Version,
                ["vnp_Command"] = "pay",
                ["vnp_TmnCode"] = _options.TmnCode,
                ["vnp_Amount"] = ((long)(amount * 100)).ToString(CultureInfo.InvariantCulture),
                ["vnp_CreateDate"] = now.ToString("yyyyMMddHHmmss"),
                ["vnp_CurrCode"] = _options.CurrencyCode,
                ["vnp_IpAddr"] = string.IsNullOrWhiteSpace(ipAddress) ? "127.0.0.1" : ipAddress,
                ["vnp_Locale"] = _options.Locale,
                ["vnp_OrderInfo"] = orderInfo,
                ["vnp_OrderType"] = _options.OrderType,
                ["vnp_ReturnUrl"] = returnUrl,
                ["vnp_TxnRef"] = orderId.ToString(CultureInfo.InvariantCulture),
                ["vnp_ExpireDate"] = now.AddMinutes(_options.ExpireMinutes).ToString("yyyyMMddHHmmss")
            };

            var hashData = BuildQuery(parameters, encode: true);
            var secureHash = HmacSha512(_options.HashSecret, hashData);
            var query = $"{hashData}&vnp_SecureHash={secureHash}";

            return $"{_options.PaymentUrl}?{query}";
        }

        public VnPayReturnResult ReadReturn(IQueryCollection query)
        {
            var values = query
                .Where(p => p.Key.StartsWith("vnp_", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(p.Key, "vnp_SecureHash", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(p.Key, "vnp_SecureHashType", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(p => p.Key, p => p.Value.ToString(), StringComparer.Ordinal);

            var sorted = new SortedDictionary<string, string>(values, StringComparer.Ordinal);
            var hashData = BuildQuery(sorted, encode: true);
            var expectedHash = HmacSha512(_options.HashSecret, hashData);
            var actualHash = query["vnp_SecureHash"].ToString();
            var responseCode = GetValue(sorted, "vnp_ResponseCode");
            var transactionStatus = GetValue(sorted, "vnp_TransactionStatus");
            var isSuccess = responseCode == "00" && transactionStatus == "00";

            decimal amount = 0;
            if (decimal.TryParse(GetValue(sorted, "vnp_Amount"), NumberStyles.Number, CultureInfo.InvariantCulture, out var rawAmount))
                amount = rawAmount / 100;

            int? orderId = null;
            if (int.TryParse(GetValue(sorted, "vnp_TxnRef"), out var parsedOrderId))
                orderId = parsedOrderId;

            var isValidSignature = IsConfigured &&
                !string.IsNullOrWhiteSpace(actualHash) &&
                string.Equals(expectedHash, actualHash, StringComparison.OrdinalIgnoreCase);

            return new VnPayReturnResult
            {
                IsValidSignature = isValidSignature,
                IsSuccess = isValidSignature && isSuccess,
                OrderId = orderId,
                ResponseCode = responseCode,
                TransactionStatus = transactionStatus,
                TransactionNo = GetValue(sorted, "vnp_TransactionNo"),
                BankCode = GetValue(sorted, "vnp_BankCode"),
                Amount = amount,
                Message = BuildMessage(isValidSignature, responseCode, transactionStatus)
            };
        }

        private static string BuildQuery(IEnumerable<KeyValuePair<string, string>> parameters, bool encode)
        {
            return string.Join("&", parameters
                .Where(p => !string.IsNullOrWhiteSpace(p.Value))
                .Select(p =>
                {
                    var value = encode ? WebUtility.UrlEncode(p.Value) : p.Value;
                    return $"{p.Key}={value}";
                }));
        }

        private static string HmacSha512(string key, string inputData)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            using var hmac = new HMACSHA512(keyBytes);
            return Convert.ToHexString(hmac.ComputeHash(inputBytes)).ToLowerInvariant();
        }

        private static string GetValue(IDictionary<string, string> values, string key)
        {
            return values.TryGetValue(key, out var value) ? value : "";
        }

        private static string BuildMessage(bool isValidSignature, string responseCode, string transactionStatus)
        {
            if (!isValidSignature)
                return "Chữ ký VNPay không hợp lệ.";

            if (responseCode == "00" && transactionStatus == "00")
                return "Thanh toán VNPay thành công.";

            return $"Thanh toán VNPay chưa thành công. Mã phản hồi: {responseCode}.";
        }
    }
}
