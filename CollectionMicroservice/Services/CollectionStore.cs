using CollectionMicroservice.Models;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionMicroservice.Services
{
    public class CollectionStore
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

        public async Task InsertCollections(IEnumerable<Collection> collections)
        {
            foreach (var collection in collections)
            {
                await _docClient.CreateDocumentAsync(_collectionsLink, collection);
            }
        }

        public IEnumerable<Collection> GetAllCollections()
        {
            return _docClient.CreateDocumentQuery<Collection>(_collectionsLink)
                                            .OrderBy(c => c.Name);
        }

        public IEnumerable<Collection> GetAllCollectionsForUser(string userId)
        {
            return _docClient.CreateDocumentQuery<Collection>(_collectionsLink)
                                            .Where(c => c.Owner == userId)
                                            .ToList()
                                            .OrderBy(c => c.Name);
        }

        public Collection GetCollection(string id)
        {
            return _docClient.CreateDocumentQuery<Collection>(_collectionsLink)
                                            .Where(c => c.Id == id)
                                            .FirstOrDefault();                                    
        }
    }
}
