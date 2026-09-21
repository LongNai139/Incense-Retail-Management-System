using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Interfaces
{
    public interface IAccountService
    {
        Customer? GetCustomerById(int id);
        Customer? Login(string phone, string password);
        bool Register(Customer data);
        bool UpdateProfile(int id, string customerName, string? email, string? address);
        string GenerateOTP();
    }
}
