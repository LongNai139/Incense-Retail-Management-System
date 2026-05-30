using SV22T1080045.Shop.Abstractions.Models.Operational;

namespace SV22T1080045.Shop.BusinessLayers.Interfaces
{
    public interface IOperationalAlertService
    {
        OperationalAlertFeed GetFeed(DateTime? since);
    }
}
