using Listable.CollectionMicroservice.DTO;
using Listable.CollectionMicroservice.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Listable.CollectionMicroservice.Tests.Mocks
{
    class MockCollectionStore : ICollectionStore
    {
        private List<Collection> _collections;

        public MockCollectionStore()
        {
            _collections = new List<Collection>();
        }

        public void ClearCollections()
        {
            _collections.Clear();
        }

        public bool DeleteCollection(string id)
        {
            return true;
        }

        public IEnumerable<Collection> GetAllCollections()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Collection> GetAllCollectionsForUser(int userId)
        {
            return _collections.Where(c => c.Owner == userId);
        }

        public Collection GetCollection(string id)
        {
            return _collections.Where(c => c.Id == id).First();
        }

        public CollectionItem GetCollectionItem(string collectionId, Guid itemId)
        {
            var collection = _collections.Where(c => c.Id == collectionId).First();

            if (collection != null)
            {
                return collection.CollectionItems.Where(i => i.Id == itemId).First();
            }

            return null; 
        }

        public Collection InsertCollection(Collection collection)
        {
            _collections.Add(collection);

            return collection;
        }

        public IEnumerable<Collection> QueryCollections(CollectionQuery query)
        {
            return _collections.Where(c => c.Name.ToLower().Contains(query.SearchTerm.ToLower())).ToList();
        }

        public bool UpdateCollection(string id, Collection updatedCollection)
        {
            return true;
        }
    }
}
