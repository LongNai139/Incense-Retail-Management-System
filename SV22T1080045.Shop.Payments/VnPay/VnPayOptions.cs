namespace SV22T1080045.Shop.Payments.VnPay
{
    public class VnPayOptions
    {
        public string Version { get; set; } = "2.1.0";
        public string PaymentUrl { get; set; } = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        public string TmnCode { get; set; } = "";
        public string HashSecret { get; set; } = "";
        public string Locale { get; set; } = "vn";
        public string CurrencyCode { get; set; } = "VND";
        public string OrderType { get; set; } = "other";
        public int ExpireMinutes { get; set; } = 15;

        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(PaymentUrl) &&
            !string.IsNullOrWhiteSpace(TmnCode) &&
            !string.IsNullOrWhiteSpace(HashSecret);
    }
}
