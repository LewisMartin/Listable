using Listable.CollectionMicroservice.DTO;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Listable.CollectionMicroservice.Services
{
    public class CollectionStore : ICollectionStore
    {
        private DocumentClient _docClient;
        private Uri _collectionsLink;
        private readonly IConfiguration _configuration;

        public CollectionStore(IConfiguration configuration)
        {
            _configuration = configuration;

            _docClient = new DocumentClient(new Uri(_configuration["CollectionDocumentDb:Uri"]), _configuration["CollectionDocumentDb:Key"]);
            _collectionsLink = UriFactory.CreateDocumentCollectionUri("listable", "collections");
        }

        public Collection InsertCollection(Collection collection)
        {
            return (dynamic)_docClient.CreateDocumentAsync(_collectionsLink, collection).Result.Resource;
        }

        public IEnumerable<Collection> GetAllCollections()
        {
            return _docClient.CreateDocumentQuery<Collection>(_collectionsLink)
                                            .OrderBy(c => c.Name);
        }

        public IEnumerable<Collection> GetAllCollectionsForUser(int userId)
        {
            return _docClient.CreateDocumentQuery<Collection>(_collectionsLink)
                                            .Where(c => c.Owner == userId)
                                            .ToList()
                                            .OrderBy(c => c.Name);
        }

        public IEnumerable<Collection> QueryCollections(CollectionQuery query)
        {
            return _docClient.CreateDocumentQuery<Collection>(_collectionsLink)
                                            .Where(c => c.Name.Contains(query.SearchTerm) && !c.PrivateMode)
                                            .Take(50)
                                            .ToList();
        }

        public Collection GetCollection(string id)
        {
            return _docClient.CreateDocumentQuery<Collection>(_collectionsLink)
                                            .Where(c => c.Id == id)
                                            .AsEnumerable()
                                            .FirstOrDefault();                                    
        }

        public CollectionItem GetCollectionItem(string collectionId, Guid itemId)
        {
            var collection = _docClient.CreateDocumentQuery<Collection>(_collectionsLink)
                                            .Where(c => c.Id == collectionId)
                                            .AsEnumerable()
                                            .FirstOrDefault();

            return collection.CollectionItems.Where(i => i.Id == itemId).AsEnumerable().FirstOrDefault();
        }

        public bool UpdateCollection(string id, Collection updatedCollection)
        {
            try
            {
                _docClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri("listable", "collections", id), updatedCollection);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteCollection(string id)
        {
            try
            {
                _docClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri("listable", "collections", id));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
