using Listable.CollectionMicroservice.DTO;
using GatewayAPI.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace GatewayAPI.Tests.Mocks
{
    public class MockCollectionsService : ICollectionsService
    {
        public List<Collection> DummyCollections { get; private set; }

        public MockCollectionsService()
        {
            DummyCollections = DefaultDummyCollections();
        }

        public MockCollectionsService(List<Collection> dummyCollections)
        {
            DummyCollections = dummyCollections;
        }

        public Task<HttpResponseMessage> Create(Collection collection)
        {
            return Task.FromResult(new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(collection))
            });
        }

        public Task<HttpResponseMessage> CreateItem(string collectionId, CollectionItem item)
        {
            return Task.FromResult(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK });
        }

        public Task<HttpResponseMessage> Update(Collection collection)
        {
            return Task.FromResult(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK });
        }

        public Task<HttpResponseMessage> Delete(string id)
        {
            return Task.FromResult(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK });
        }

        public Task<HttpResponseMessage> DeleteItem(string collectionId, string content)
        {
            return Task.FromResult(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK });
        }

        public Task<HttpResponseMessage> Retrieve(string collectionId)
        {
            return Task.FromResult(new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(DummyCollections.FirstOrDefault(c => c.Id == collectionId)))
            });
        }

        public Task<HttpResponseMessage> RetrieveAll(string userId)
        {
            return Task.FromResult(new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(DummyCollections))
            });
        }

        private List<Collection> DefaultDummyCollections()
        {
            return new List<Collection>()
            {
                new Collection()
                {
                    Id = "1",
                    Owner = "TestUser",
                    Name = "Collection 1",
                    ImageEnabled = false,
                    DisplayFormat = CollectionDisplayFormat.List,
                    CollectionItems = new List<CollectionItem>()
                    {
                        new CollectionItem()
                        {
                            Id = Guid.NewGuid(),
                            Name = "Item 1",
                            Description = "Item 1 description",
                            ImageId = null
                        }
                    }
                },
                new Collection()
                {
                    Id = "2",
                    Owner = "TestUser",
                    Name = "Collection 2",
                    ImageEnabled = true,
                    DisplayFormat = CollectionDisplayFormat.Grid,
                    CollectionItems = new List<CollectionItem>()
                    {
                        new CollectionItem()
                        {
                            Id = Guid.NewGuid(),
                            Name = "Item 1",
                            Description = "Item 1 description",
                            ImageId = "1"
                        }
                    }
                }
            };
        }

        public Task<HttpResponseMessage> UpdateItem(string collectionId, CollectionItem item)
        {
            return Task.FromResult(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK });
        }

        public Task<HttpResponseMessage> RetrieveItem(string collectionId, string itemId)
        {
            return Task.FromResult(new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(DummyCollections.FirstOrDefault(c => c.Id == collectionId).CollectionItems.FirstOrDefault(i => i.Id.ToString() == itemId)))
            });
        }
    }
}
