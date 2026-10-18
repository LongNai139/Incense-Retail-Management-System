using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    public interface ICustomerDAL
    {
        Customer? GetById(int id);
        Customer? GetByPhone(string phone);
        Customer? Authenticate(string phone, string password);
        List<Customer> ListForPermissionAssignment();
        int Add(Customer customer);
        bool UpdateProfile(Customer customer);
    }
}
