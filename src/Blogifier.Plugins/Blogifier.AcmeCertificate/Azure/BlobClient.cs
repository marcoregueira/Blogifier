using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Azure.Storage.Blobs;

using Blogifier.Plugin.AcmeCertificate.Config;

using Microsoft.Extensions.Options;

namespace Blogifier.Plugin.AcmeCertificate.Azure
{
    public class BlobClient
    {
        private BlobContainerClient _blobContainerClient;
        private BlobServiceClient _blobServiceClient;
        private AzureBlobStorageConfig _options;

        public BlobClient(IOptions<AzureBlobStorageConfig> options)
        {
            _options = options.Value;
            _blobServiceClient = new BlobServiceClient(_options.ConnectionString);
        }

        public BlobClient(AzureBlobStorageConfig options)
        {
            _options = options;
            _blobServiceClient = new BlobServiceClient(_options.ConnectionString);
        }

        public async Task<BlobContainerClient> GetContainerClient(string containerName)
        {
            if (_blobContainerClient != null) return _blobContainerClient;

            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await _blobContainerClient.CreateIfNotExistsAsync();

            return _blobContainerClient;
        }

        public async Task<string> ReadAsync(string name, CancellationToken cancellationToken)
        {
            var token = Encoding.UTF8.GetString(await ReadBinaryAsync(name, cancellationToken));
            return token;
        }

        public async Task<BinaryData> ReadBinaryAsync(string name, CancellationToken cancellationToken)
        {
            var container = await GetContainerClient(_options.ContainerName);
            var client = container.GetBlobClient(name);
            if (client.Exists())
            {
                var blob = await client.DownloadContentAsync(cancellationToken);
                return blob.Value.Content;
            }
            return null;
        }

        public Task SaveAsync(string name, string value, CancellationToken cancellationToken)
                            => SaveAsync(name, Encoding.UTF8.GetBytes(value), cancellationToken);

        public async Task SaveAsync(string name, byte[] value, CancellationToken cancellationToken)
        {
            var container = await GetContainerClient(_options.ContainerName);
            await container.UploadBlobAsync(name, new MemoryStream(value), cancellationToken);
        }
    }
}
