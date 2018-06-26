using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Listable.MVCWebApp.Services
{
    public interface IBlobService
    {
        Task<string> ImageUpload(MultipartFormDataContent content);

        void ImageDelete(string uriParams);

        Task<string> ImageRetrieveUrl(string uriParams);

        Task<Dictionary<string, string>> ImageRetrieveThumbs(string uriParams);
    }
}