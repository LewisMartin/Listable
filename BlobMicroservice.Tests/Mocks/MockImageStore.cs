using Listable.BlobMicroservice.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Listable.BlobMicroservice.Tests.Mocks
{
    public class MockImageStore : IImageStore
    {
        public async Task<bool> DeleteImage(string imageId)
        {
            return true;
        }

        public async Task<bool> DeleteThumb(string imageId)
        {
            return true;
        }

        public string GetUri(string imageId)
        {
            return "http://localhost/" + imageId;
        }

        public Dictionary<string, string> MapThumbnailUris(string[] imageIds)
        {
            return new Dictionary<string, string>()
            {
                { "1", "http://localhost/1" }
            };
        }

        public async Task<string> SaveImage(Stream stream)
        {
            return Guid.NewGuid().ToString();
        }
    }
}
