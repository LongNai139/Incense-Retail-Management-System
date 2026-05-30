using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Interfaces
{
    public interface IUnitService
    {
        List<Unit> ListUnits(int take = 0);
        Unit? GetById(int id);
    }
}
