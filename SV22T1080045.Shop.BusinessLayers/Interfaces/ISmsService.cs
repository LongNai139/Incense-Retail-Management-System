namespace SV22T1080045.Shop.BusinessLayers.Interfaces
{
    public interface ISmsService
    {
        bool Send(string phone, string message);
    }
}
