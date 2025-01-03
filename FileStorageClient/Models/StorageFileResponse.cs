namespace FileStorageClient.Models
{
    public class StorageFileResponse : FileResponse
    {
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public string? Base64Content { get; set; }
        public byte[]? RawData { get; set; }
    }
}
