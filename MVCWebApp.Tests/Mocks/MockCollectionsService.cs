using Listable.CollectionMicroservice.DTO;
using Listable.MVCWebApp.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Listable.MVCWebApp.Tests.Mocks
{
    public class MockCollectionsService : ICollectionsService
    {
        private List<Collection> _DummyCollections;

        public MockCollectionsService()
        {
            _DummyCollections = SetDummyCollections();
        }

        public Task<HttpResponseMessage> Create(Collection collection)
        {
            return Task.FromResult(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK });
        }

        public Task<HttpResponseMessage> CreateItem(string collectionId, CollectionItem item)
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
                Content = new StringContent(JsonConvert.SerializeObject(_DummyCollections.FirstOrDefault(c => c.Id == collectionId)))
            });
        }

        public Task<HttpResponseMessage> RetrieveAll(string userId)
        {
            return Task.FromResult(new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(_DummyCollections))
            });
        }

        private List<Collection> SetDummyCollections()
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
    }
}
