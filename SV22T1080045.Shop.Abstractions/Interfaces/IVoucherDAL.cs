using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    public interface IVoucherDAL
    {
        Voucher? GetByCode(string code);
        bool Use(string code);
        int Add(Voucher v);
        bool Update(Voucher v);
        bool Delete(int id);
    }
}
