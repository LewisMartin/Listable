using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BlobMicroservice.Services
{
    public interface IImageStore
    {
        Task<string> SaveImage(Stream stream);

        Task<bool> DeleteImage(string imageId);

        Task<bool> DeleteThumb(string imageId);

        string GetUri(string imageId);

        Dictionary<string, string> MapThumbnailUris(string[] imageIds);
    }
}