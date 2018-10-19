using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GatewayAPI.Services
{
    public interface IBlobService
    {
        Task<HttpResponseMessage> ImageUpload(MultipartFormDataContent content);

        Task<HttpResponseMessage> ImageUpdate(string imgId, MultipartFormDataContent content);

        Task<HttpResponseMessage> ImageDelete(string imgId);

        Task<HttpResponseMessage> ImageRetrieveUrl(string imgId);

        Task<HttpResponseMessage> ImageRetrieveThumbs(List<string> imgIds);
    }
}
