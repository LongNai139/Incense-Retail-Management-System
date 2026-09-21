namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    public interface IPhoneOtpDAL
    {
        string Generate(string phone, string purpose);
        bool Verify(string phone, string purpose, string code);
        void CleanupExpired();
    }
}
