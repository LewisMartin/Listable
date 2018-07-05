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
        public Task<bool> DeleteImage(string imageId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteThumb(string imageId)
        {
            throw new NotImplementedException();
        }

        public string GetUri(string imageId)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> MapThumbnailUris(string[] imageIds)
        {
            throw new NotImplementedException();
        }

        public Task<string> SaveImage(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
