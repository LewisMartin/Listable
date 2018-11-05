using Listable.CollectionMicroservice.DTO;
using System;
using System.Collections.Generic;

namespace Listable.CollectionMicroservice.Services
{
    public interface ICollectionStore
    {
        Collection InsertCollection(Collection collection);

        IEnumerable<Collection> GetAllCollections();

        IEnumerable<Collection> GetAllCollectionsForUser(string userId);

        Collection GetCollection(string id);

        CollectionItem GetCollectionItem(string collectionId, Guid itemId);

        bool UpdateCollection(string id, Collection updatedCollection);

        bool DeleteCollection(string id);
    }
}