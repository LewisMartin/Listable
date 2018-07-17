using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

namespace Listable.MVCWebApp.Services
{
    public enum BlobApiAction
    {
        ImageUpload,
        ImageDelete,
        ImageRetrieveUrl,
        ImageRetrieveThumbs
    }

    public class BlobService : BackendService<BlobApiAction>, IBlobService
    {
        public BlobService(IConfiguration configuration, IDistributedCache cache, IHttpContextAccessor accessor) : base(configuration, cache, accessor)
        {
        }

        public async Task<HttpResponseMessage> ImageUpload(MultipartFormDataContent content)
        {
            return await APIRequest(BlobApiAction.ImageUpload, "", content);
        }

        public async Task<HttpResponseMessage> ImageDelete(string imgId)
        {
            return await APIRequest(BlobApiAction.ImageDelete, "?id=" + imgId);
        }

        public async Task<HttpResponseMessage> ImageRetrieveUrl(string imgId)
        {
            return await APIRequest(BlobApiAction.ImageRetrieveUrl, "?id=" + imgId);
        }

        public async Task<HttpResponseMessage> ImageRetrieveThumbs(List<string> imgIds)
        {
            string imageIds = "";
            foreach (var id in imgIds)
            {
                if (id != null && id != "")
                {
                    if (imageIds != "")
                        imageIds += "&";

                    imageIds += ("ids=" + id);
                }
            }

            return await APIRequest(BlobApiAction.ImageRetrieveThumbs, "?" + imageIds);
        }

        protected override async Task<HttpResponseMessage> APIRequest(BlobApiAction action, string uriParams = "", HttpContent content = null)
        {
            var req = FormAPIRequestMessage(action, uriParams);

            if (action == BlobApiAction.ImageUpload && content != null)
                req.Content = content;

            string accessToken = await GetAccessTokenAsync(ListableAPI.BlobAPI);
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            return await Client.SendAsync(req);
        }

        protected override HttpRequestMessage FormAPIRequestMessage(BlobApiAction action, string uriParams)
        {
            switch (action)
            {
                case BlobApiAction.ImageUpload:
                    return new HttpRequestMessage(HttpMethod.Post, (_configuration["BlobServiceAPI:APIEndpoint"] + "/upload" + uriParams));
                case BlobApiAction.ImageRetrieveUrl:
                    return new HttpRequestMessage(HttpMethod.Get, (_configuration["BlobServiceAPI:APIEndpoint"] + "/retrieveurl" + uriParams));
                case BlobApiAction.ImageRetrieveThumbs:
                    return new HttpRequestMessage(HttpMethod.Get, (_configuration["BlobServiceAPI:APIEndpoint"] + "/retrievethumbnailurls" + uriParams));
                case BlobApiAction.ImageDelete:
                    return new HttpRequestMessage(HttpMethod.Delete, (_configuration["BlobServiceAPI:APIEndpoint"] + "/delete" + uriParams));
                default:
                    return new HttpRequestMessage();
            }
        }
    }
}
