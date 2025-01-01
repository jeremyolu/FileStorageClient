using FileClient.Interfaces;
using FileClient.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FileStorageClient.Extensions
{
    public static class FileStorageExtensions
    {
        public static IServiceCollection AddStorageFileClient(this IServiceCollection services, string connectionString)
        {
            return services.AddSingleton<IStorageClient>(client => new StorageClient(connectionString));
        }
    }
}
