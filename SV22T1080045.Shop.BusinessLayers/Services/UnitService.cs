using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.BusinessLayers.Interfaces;
using SV22T1080045.Shop.DomainModels;

namespace SV22T1080045.Shop.BusinessLayers.Services
{
    public class UnitService : IUnitService
    {
        private readonly IUnitDAL _unitDal;

        public UnitService(IUnitDAL unitDal)
        {
            _unitDal = unitDal;
        }

        public List<Unit> ListUnits(int take = 0)
            => _unitDal.ListUnits(take);

        public Unit? GetById(int id)
            => _unitDal.GetById(id);
    }
}
