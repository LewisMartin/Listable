using Listable.CollectionMicroservice.DTO;
using Listable.CollectionMicroservice.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Listable.CollectionMicroservice.Tests.Mocks
{
    class MockCollectionStore : ICollectionStore
    {
        public bool DeleteCollection(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Collection> GetAllCollections()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Collection> GetAllCollectionsForUser(string userId)
        {
            throw new NotImplementedException();
        }

        public Collection GetCollection(string id)
        {
            throw new NotImplementedException();
        }

        public CollectionItem GetCollectionItem(string collectionId, Guid itemId)
        {
            throw new NotImplementedException();
        }

        public Task InsertCollections(IEnumerable<Collection> collections)
        {
            throw new NotImplementedException();
        }

        public bool UpdateCollection(string id, Collection updatedCollection)
        {
            throw new NotImplementedException();
        }
    }
}
