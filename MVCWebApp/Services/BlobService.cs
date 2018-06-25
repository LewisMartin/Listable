using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Listable.MVCWebApp.Services
{
    public enum BlobApiAction
    {
        ImageUpload,
        ImageDelete,
        ImageRetrieveUrl,
        ImageRetrieveThumbs
    }

    public class BlobService : BackendService<BlobApiAction>
    {
        public BlobService(IConfiguration configuration, IDistributedCache cache, IHttpContextAccessor accessor) : base(configuration, cache, accessor)
        {
        }

        public async Task<string> ImageUpload(MultipartFormDataContent content)
        {
            HttpResponseMessage blobRes = await APIRequest(BlobApiAction.ImageUpload, "", content);
            return await blobRes.Content.ReadAsStringAsync();
        }

        public async void ImageDelete(string uriParams)
        {
            HttpResponseMessage blobRes = await APIRequest(BlobApiAction.ImageDelete, uriParams);
        }

        public async Task<string> ImageRetrieveUrl(string uriParams)
        {
            HttpResponseMessage blobRes = await APIRequest(BlobApiAction.ImageRetrieveUrl, uriParams);
            return await blobRes.Content.ReadAsStringAsync();
        }

        public async Task<Dictionary<string, string>> ImageRetrieveThumbs(string uriParams)
        {
            HttpResponseMessage res = await APIRequest(BlobApiAction.ImageRetrieveThumbs, uriParams);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(await res.Content.ReadAsStringAsync());
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
