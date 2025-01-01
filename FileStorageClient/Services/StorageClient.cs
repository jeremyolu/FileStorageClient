using Azure.Storage.Blobs;
using FileClient.Interfaces;
using FileClient.Models;

namespace FileClient.Services
{
    public class StorageClient : IStorageClient
    {
        private BlobServiceClient? _blobServiceClient = null;

        public StorageClient(string connectionString)
        {
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

                string containerName = segments[0];
                string blobPath = string.Join('/', segments[1..]);

                try
                {
                    var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                    var blobClient = containerClient.GetBlobClient(blobPath);

                    result = await blobClient.ExistsAsync();
                }
                catch (Exception ex) { }

            }

            return result;
        }
    }
}
