using SV22T1080045.Shop.Abstractions.Models;

namespace SV22T1080045.Shop.Abstractions.Interfaces
{
    public interface IFileStorageService
    {
        string SaveFile(FileUploadData file, string folder);
        void DeleteFile(string folder, string fileName);
    }
}
