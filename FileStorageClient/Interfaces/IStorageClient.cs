using FileClient.Models;
using FileStorageClient.Models;

namespace FileClient.Interfaces
{
    public interface IStorageClient
    {
        Task<bool> UploadFileAsync(FileType? fileType, string path, Stream fileStream, Action? onUpload = null);
        Task<StorageFileResponse> GetFileAsync(FileType? fileType, string path, Action<StorageFileResponse>? onFoundFile = null);
        Task<bool> FileExistsAsync(FileType? fileType, string path);
        Task<FileResponse> DeleteFileAsync(FileType? fileType, string path, Action? onDelete = null);
    }
}
