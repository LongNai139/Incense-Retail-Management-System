namespace SV22T1080045.Shop.Payments.MoMo
{
    public class MoMoPaymentResult
    {
        public bool IsSuccess { get; set; }
        public string? PayUrl { get; set; }
        public string Message { get; set; } = "";
        public int ResultCode { get; set; }
    }

    public class MoMoIpnResult
    {
        public bool IsValidSignature { get; set; }
        public bool IsSuccess { get; set; }
        public int? OrderId { get; set; }
        public string Message { get; set; } = "";
        public long TransId { get; set; }
        public string? RequestId { get; set; }
    }
}
