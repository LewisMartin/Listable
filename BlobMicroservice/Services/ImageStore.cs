using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
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
            bool success = await blob.DeleteIfExistsAsync();

            if (success)
                return await DeleteThumb(imageId);
            else
                return success;
        }

        public async Task<bool> DeleteThumb(string imageId)
        {
            var container = _blobClient.GetContainerReference("thumbs");
            var blob = container.GetBlockBlobReference("sm-" + imageId);
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

            return _configuration["BlobStorage:ImageUrl"] + "/" + imageId + sas;
        }

        public Dictionary<string, string> MapThumbnailUris(string[] imageIds)
        {
            var sasPolicy = new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-15),
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(15)
            };

            Dictionary<string, string> thumbnailMap = new Dictionary<string, string>();

            foreach (string imageId in imageIds)
            {
                if (imageId != null && imageId != "")
                {
                    var container = _blobClient.GetContainerReference("thumbs");
                    var blob = container.GetBlockBlobReference("sm-" + imageId);
                    var sas = blob.GetSharedAccessSignature(sasPolicy);
                    thumbnailMap.Add(imageId, _configuration["BlobStorage:ThumbnailUrl"] + "/sm-" + imageId + sas);
                }
            }

            return thumbnailMap;
        }
    }
}
