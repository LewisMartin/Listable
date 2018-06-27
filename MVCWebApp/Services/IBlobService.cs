using System.Net.Http;
using System.Threading.Tasks;

namespace Listable.MVCWebApp.Services
{
    public interface IBlobService
    {
        Task<HttpResponseMessage> ImageUpload(MultipartFormDataContent content);

        Task<HttpResponseMessage> ImageDelete(string uriParams);

        Task<HttpResponseMessage> ImageRetrieveUrl(string uriParams);

        Task<HttpResponseMessage> ImageRetrieveThumbs(string uriParams);
    }
}