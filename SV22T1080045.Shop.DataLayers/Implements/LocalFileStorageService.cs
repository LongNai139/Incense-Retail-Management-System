using Microsoft.AspNetCore.Hosting;
using SV22T1080045.Shop.Abstractions.Interfaces;
using SV22T1080045.Shop.Abstractions.Models;

namespace SV22T1080045.Shop.DataLayers.Implements
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public LocalFileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public string SaveFile(FileUploadData file, string folder)
        {
            if (!file.HasContent)
                throw new InvalidOperationException("Tệp tải lên không có dữ liệu.");

            var originalFileName = Path.GetFileName(file.FileName);
            var fileName = $"{DateTime.Now.Ticks}_{originalFileName}";
            var folderPath = Path.Combine(_environment.WebRootPath, NormalizeFolder(folder));
            Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);
            File.WriteAllBytes(filePath, file.Content);

            return fileName;
        }

        public void DeleteFile(string folder, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;

            var filePath = Path.Combine(_environment.WebRootPath, NormalizeFolder(folder), fileName);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        private static string NormalizeFolder(string folder)
        {
            return folder
                .Replace('\\', '/')
                .Trim('/')
                .Replace("wwwroot/", "", StringComparison.OrdinalIgnoreCase);
        }
    }
}
