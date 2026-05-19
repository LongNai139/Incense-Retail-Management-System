using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Interfaces
{
    public interface IAccountService
    {
        Customer? Login(string phone, string password);
        bool Register(Customer data);
        string GenerateOTP();
    }
}
