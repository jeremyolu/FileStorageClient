using FileClient.Models;
using FileStorageClient.Models;

namespace FileClient.Interfaces
{
    public interface IStorageClient
    {
        Task<bool> FileExists(FileType? fileType, string path);
        Task<FileResponse> DeleteFile(FileType? fileType, string path);
    }
}
