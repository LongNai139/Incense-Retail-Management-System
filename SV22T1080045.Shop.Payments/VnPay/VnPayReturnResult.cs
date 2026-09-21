namespace SV22T1080045.Shop.Payments.VnPay
{
    public class VnPayReturnResult
    {
        public bool IsValidSignature { get; set; }
        public bool IsSuccess { get; set; }
        public int? OrderId { get; set; }
        public string ResponseCode { get; set; } = "";
        public string TransactionStatus { get; set; } = "";
        public string? TransactionNo { get; set; }
        public string? BankCode { get; set; }
        public decimal Amount { get; set; }
        public string Message { get; set; } = "";
    }
}
