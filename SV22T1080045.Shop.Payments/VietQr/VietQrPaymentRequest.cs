namespace SV22T1080045.Shop.Payments.VietQr
{
    public class VietQrPaymentRequest
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string TransferContent { get; set; } = "";
    }

    public class VietQrPaymentInfo
    {
        public string QrImageUrl { get; set; } = "";
        public string BankId { get; set; } = "";
        public string AccountNumber { get; set; } = "";
        public string AccountName { get; set; } = "";
        public decimal Amount { get; set; }
        public string TransferContent { get; set; } = "";
        public string OrderCode { get; set; } = "";
    }
}
