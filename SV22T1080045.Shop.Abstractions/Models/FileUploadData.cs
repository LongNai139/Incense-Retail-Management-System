namespace SV22T1080045.Shop.Abstractions.Models
{
    public class FileUploadData
    {
        public string FileName { get; set; } = "";
        public string ContentType { get; set; } = "";
        public byte[] Content { get; set; } = Array.Empty<byte>();

        public bool HasContent => Content.Length > 0;
    }
}
