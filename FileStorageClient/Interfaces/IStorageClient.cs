using FileClient.Models;

namespace FileClient.Interfaces
{
    public interface IStorageClient
    {
        Task<bool> FileExists(FileType? fileType, string path);
    }
}
