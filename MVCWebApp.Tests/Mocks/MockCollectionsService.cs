using Listable.CollectionMicroservice.DTO;
using Listable.MVCWebApp.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Listable.MVCWebApp.Tests.Mocks
{
    class MockCollectionsService : ICollectionsService
    {
        public Task<HttpResponseMessage> Create(Collection collection)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> CreateItem(string collectionId, CollectionItem item)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> Delete(string uriParams)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> DeleteItem(string collectionId, string content)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> Retrieve(string uriParams)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> RetrieveAll(string uriParams)
        {
            throw new NotImplementedException();
        }
    }
}
