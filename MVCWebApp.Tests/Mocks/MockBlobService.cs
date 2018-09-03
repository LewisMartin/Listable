using Listable.MVCWebApp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Listable.MVCWebApp.Tests.Mocks
{
    public class MockBlobService : IBlobService
    {
        public Task<HttpResponseMessage> ImageDelete(string imgId)
        {
            return Task.FromResult(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK });
        }

        public Task<HttpResponseMessage> ImageRetrieveThumbs(List<string> imgIds)
        {
            Dictionary<string, string> thumbnailMap = new Dictionary<string, string>();

            foreach (string imageId in imgIds)
            {
                if (imageId != null && imageId != "")
                {
                    thumbnailMap.Add(imageId, "https://blobcontainerurl/" + imageId);
                }
            }

            return Task.FromResult(new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(thumbnailMap))
            });
        }

        public Task<HttpResponseMessage> ImageRetrieveUrl(string imgId)
        {
            return Task.FromResult(new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject("https://blobcontainerurl/" + imgId))
            });
        }

        public Task<HttpResponseMessage> ImageUpload(MultipartFormDataContent content)
        {
            return Task.FromResult(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK });
        }

        public Task<HttpResponseMessage> ImageUpdate(string imgId, MultipartFormDataContent content)
        {
            throw new NotImplementedException();
        }
    }
}
