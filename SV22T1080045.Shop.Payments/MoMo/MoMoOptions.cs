namespace SV22T1080045.Shop.Payments.MoMo
{
    public class MoMoOptions
    {
        public string Endpoint { get; set; } = "https://test-payment.momo.vn/v2/gateway/api/create";
        public string PartnerCode { get; set; } = "";
        public string AccessKey { get; set; } = "";
        public string SecretKey { get; set; } = "";
        public string StoreId { get; set; } = "";
        public string PartnerName { get; set; } = "Tram Huong Shop";
        public string RequestType { get; set; } = "payWithMethod";
        public string Lang { get; set; } = "vi";
        public string ExtraData { get; set; } = "";

        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(Endpoint) &&
            !string.IsNullOrWhiteSpace(PartnerCode) &&
            !string.IsNullOrWhiteSpace(AccessKey) &&
            !string.IsNullOrWhiteSpace(SecretKey);
    }
}
