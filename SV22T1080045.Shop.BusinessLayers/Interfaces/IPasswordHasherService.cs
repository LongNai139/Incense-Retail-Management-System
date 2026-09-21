namespace SV22T1080045.Shop.BusinessLayers.Interfaces
{
    public interface IPasswordHasherService
    {
        string Hash(string plainTextPassword);
        bool Verify(string plainTextPassword, string storedHash);
    }
}
