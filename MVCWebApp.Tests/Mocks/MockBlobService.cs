using Listable.MVCWebApp.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Listable.MVCWebApp.Tests.Mocks
{
    class MockBlobService : IBlobService
    {
        public Task<HttpResponseMessage> ImageDelete(string uriParams)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> ImageRetrieveThumbs(string uriParams)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> ImageRetrieveUrl(string uriParams)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> ImageUpload(MultipartFormDataContent content)
        {
            throw new NotImplementedException();
        }
    }
}
