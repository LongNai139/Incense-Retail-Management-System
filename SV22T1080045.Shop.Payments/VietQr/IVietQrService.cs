namespace SV22T1080045.Shop.Payments.VietQr
{
    public interface IVietQrService
    {
        bool IsConfigured { get; }
        VietQrPaymentInfo BuildPaymentInfo(VietQrPaymentRequest request);
    }
}
