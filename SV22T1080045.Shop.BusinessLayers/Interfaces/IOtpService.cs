namespace SV22T1080045.Shop.BusinessLayers.Interfaces
{
    public interface IOtpService
    {
        bool Send(string phone, string purpose);
        bool Verify(string phone, string purpose, string code);
    }
}
