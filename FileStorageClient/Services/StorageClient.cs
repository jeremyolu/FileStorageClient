using Azure.Storage.Blobs;
using FileClient.Interfaces;
using FileClient.Models;
using FileStorageClient.Models;

namespace FileClient.Services
{
    public class StorageClient : IStorageClient
    {
        private static string? _connectionString;
        private BlobServiceClient? _blobServiceClient = null;

        public StorageClient(string connectionString)
        {
            _connectionString = connectionString;
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<bool> FileExists(FileType? fileType, string path)
        {
            bool result = false;

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("Storage file path is currently null", nameof(path));

            if (fileType == null || fileType.Value == FileType.Disk)
            {
                result = File.Exists(path);
            }
            else
            {
                if (_blobServiceClient == null)
                    throw new Exception("blob service client is null");

                string[] segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                string container = segments[0];
                string blobPath = string.Join('/', segments[1..]);

                try
                {
                    var containerClient = _blobServiceClient.GetBlobContainerClient(container);
                    var blobClient = containerClient.GetBlobClient(blobPath);

                    result = await blobClient.ExistsAsync();
                }
                catch (Exception) { }

            }

            return result;
        }

        public async Task<FileResponse> DeleteFile(FileType? fileType, string path)
        {
            var response = new FileResponse { Status = true };

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("Storage file path is currently null", nameof(path));

            if (fileType == null || fileType.Value == FileType.Disk)
            {
                try 
                {   if (!File.Exists(path))
                    {
                        response.Message = "The file does not exist on the local disk.";
                    }
                    else
                    {
                        File.Delete(path);
                        response.Message = "The file has been successfully removed from the local disk.";
                    }
                }
                catch (Exception ex) { response.Status = false; response.Message = ex.Message; }
            }
            else
            {
                if (_blobServiceClient == null)
                    throw new Exception("blob service client is null");

                string[] segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                string container = segments[0];
                string blobPath = string.Join('/', segments[1..]);

                try
                {
                    var containerClient = new BlobContainerClient(_connectionString, container);
                    if (!await containerClient.ExistsAsync())
                    { 
                        response.Status = false; 
                        response.Message = "The container does not exist in the storage client."; 
                        return response;
                    }

                    var blobClient = containerClient?.GetBlobClient(blobPath);
                    if (!await blobClient?.ExistsAsync()!)
                    {
                        response.Message = "The file does not exist in the storage client.";
                        return response;
                    }

                    await containerClient?.DeleteBlobAsync(blobPath)!;

                    response.Message = "The file has been successfully removed from the storage client.";
                }
                catch (Exception) { }
            }

            return response;
        }
    }
}
