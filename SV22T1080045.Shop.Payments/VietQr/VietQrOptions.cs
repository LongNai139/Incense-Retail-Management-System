namespace SV22T1080045.Shop.Payments.VietQr
{
    /// <summary>
    /// Cấu hình VietQR (chuyển khoản ngân hàng). BankId là mã BIN (vd: 970436 Vietcombank).
    /// </summary>
    public class VietQrOptions
    {
        public string BankId { get; set; } = "";
        public string AccountNumber { get; set; } = "";
        public string AccountName { get; set; } = "";
        public string Template { get; set; } = "compact2";
        public string ImageBaseUrl { get; set; } = "https://img.vietqr.io/image";

        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(BankId) &&
            !string.IsNullOrWhiteSpace(AccountNumber) &&
            !string.IsNullOrWhiteSpace(AccountName);
    }
}
