using Listable.CollectionMicroservice.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Listable.MVCWebApp.Services
{
    public interface ICollectionsService
    {
        Task<Collection> Retrieve(string uriParams);

        Task<List<Collection>> RetrieveAll(string uriParams);

        void Create(Collection collection);

        void Delete(string uriParams);

        void CreateItem(string collectionId, CollectionItem item);

        void DeleteItem(string collectionId, string content);
    }
}