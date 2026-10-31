using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    public interface IUnitDAL
    {
        List<Unit> ListUnits(int take = 0);
        Unit? GetById(int id);
        int Add(Unit unit);
        bool Update(Unit unit);
        bool Delete(int id);
    }
}
