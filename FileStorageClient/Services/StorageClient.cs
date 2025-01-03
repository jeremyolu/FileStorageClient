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

        public async Task<StorageFileResponse> GetFile(FileType? fileType, string path)
        {
            var response = new StorageFileResponse{ Status = false };

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("Storage file path is currently null", nameof(path));

            if (fileType == null || fileType.Value == FileType.Disk)
            {
                try
                {
                    if (!File.Exists(path))
                    {
                        response.Message = "The file does not exist on the local disk.";
                        return response;
                    }

                    var fileBytes = await File.ReadAllBytesAsync(path);

                    return new StorageFileResponse
                    {
                        FileName = Path.GetFileName(path),
                        ContentType = GetContentType(path),
                        Base64Content = Convert.ToBase64String(fileBytes),
                        RawData = await File.ReadAllBytesAsync(path),
                        Status = true,
                        Message = "File successfully retrieved from local disk."
                    };
                }
                catch (Exception ex) { }
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

                    var blob = await blobClient.DownloadContentAsync();
                    var blobBytes = blob.Value.Content.ToArray();
                    var blobProperties = await blobClient.GetPropertiesAsync();

                    return new StorageFileResponse
                    {
                        FileName = blobClient.Name,
                        ContentType = blobProperties.Value.ContentType,
                        Base64Content = Convert.ToBase64String(blobBytes),
                        RawData = blobBytes,
                        Status = true,
                        Message = "File successfully retrieved from the storage client."
                    };
                }
                catch (Exception) { }
            }

            return response;
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

        private string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".txt" => "text/plain",
                ".png" => "image/png",
                ".pdf" => "application/pdf",
                ".doc" or ".docx" => "application/msword",
                _ => "application/octet-stream"
            };
        }
    }
}
