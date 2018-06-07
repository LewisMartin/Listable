using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BlobMicroservice.Services
{
    public class ImageStore
    {
        private CloudBlobClient _blobClient;
        private string baseUri = "https://listable.blob.core.windows.net";
        private readonly IConfiguration _configuration;

        public ImageStore(IConfiguration configuration)
        {
            _configuration = configuration;

            var credentials = new StorageCredentials(_configuration["BlobStorage:StorageAccount"], _configuration["BlobStorage:StorageKey"]);
            _blobClient = new CloudBlobClient(new Uri(baseUri), credentials);
        }

        public async Task<string> SaveImage(Stream stream)
        {
            var imageId = Guid.NewGuid().ToString();
            var container = _blobClient.GetContainerReference("images");
            var blob = container.GetBlockBlobReference(imageId);
            await blob.UploadFromStreamAsync(stream);

            return imageId;
        }

        public async Task<bool> DeleteImage(string imageId)
        {
            var container = _blobClient.GetContainerReference("images");
            var blob = container.GetBlockBlobReference(imageId);
            return await blob.DeleteIfExistsAsync();
        }

        public string GetUri(string imageId)
        {
            var sasPolicy = new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-15),
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(15)
            };

            var container = _blobClient.GetContainerReference("images");
            var blob = container.GetBlockBlobReference(imageId);
            var sas = blob.GetSharedAccessSignature(sasPolicy);

            return _configuration["BlobStorage:BaseUrl"] + "/" + imageId + sas;
        }
    }
}
