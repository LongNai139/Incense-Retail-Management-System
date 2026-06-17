using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net;

namespace SV22T1080045.Shop.Payments.VietQr
{
    public class VietQrService : IVietQrService
    {
        private readonly VietQrOptions _options;

        public VietQrService(IOptions<VietQrOptions> options)
        {
            _options = options.Value;
        }

        public bool IsConfigured => _options.IsConfigured;

        public VietQrPaymentInfo BuildPaymentInfo(VietQrPaymentRequest request)
        {
            if (!IsConfigured)
                throw new InvalidOperationException("VietQR chưa được cấu hình BankId/AccountNumber/AccountName.");

            var orderCode = $"TH{request.OrderId:D6}";
            var transferContent = string.IsNullOrWhiteSpace(request.TransferContent)
                ? orderCode
                : request.TransferContent.Trim();

            var amount = (long)Math.Round(request.Amount, 0, MidpointRounding.AwayFromZero);
            var template = string.IsNullOrWhiteSpace(_options.Template) ? "compact2" : _options.Template.Trim();
            var baseUrl = _options.ImageBaseUrl.TrimEnd('/');
            var path = $"{baseUrl}/{_options.BankId}-{_options.AccountNumber}-{template}.png";

            var query = new Dictionary<string, string>
            {
                ["amount"] = amount.ToString(CultureInfo.InvariantCulture),
                ["addInfo"] = transferContent,
                ["accountName"] = _options.AccountName
            };

            var qrUrl = path + "?" + string.Join("&", query.Select(p =>
                $"{p.Key}={WebUtility.UrlEncode(p.Value)}"));

            return new VietQrPaymentInfo
            {
                QrImageUrl = qrUrl,
                BankId = _options.BankId,
                AccountNumber = _options.AccountNumber,
                AccountName = _options.AccountName,
                Amount = amount,
                TransferContent = transferContent,
                OrderCode = orderCode
            };
        }
    }
}
