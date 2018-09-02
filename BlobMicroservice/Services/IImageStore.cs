using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Listable.BlobMicroservice.Services
{
    public interface IImageStore
    {
        Task<string> SaveImage(Stream stream);

        Task<string> SaveImage(Stream stream, string imageId);

        Task<bool> DeleteImage(string imageId);

        Task<bool> DeleteThumb(string imageId);

        string GetUri(string imageId);

        Dictionary<string, string> MapThumbnailUris(string[] imageIds);
    }
}