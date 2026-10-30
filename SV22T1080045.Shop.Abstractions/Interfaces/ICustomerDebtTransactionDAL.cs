using SV22T1080045.Shop.DomainModels.Warehouse;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    public interface ICustomerDebtTransactionDAL
    {
        List<CustomerDebtTransaction> ListTransactions(int customerDebtId);
        CustomerDebtTransaction? GetById(int id);
        int Add(CustomerDebtTransaction transaction);
        bool Delete(int id);
    }
}
