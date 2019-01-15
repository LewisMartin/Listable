using System.Net.Http;
using System.Threading.Tasks;
using Listable.CollectionMicroservice.DTO;

namespace GatewayAPI.Services
{
    public interface ICollectionsService
    {
        Task<HttpResponseMessage> Retrieve(string collectionId);

        Task<HttpResponseMessage> RetrieveAll(int userId);

        Task<HttpResponseMessage> RetrieveItem(string collectionId, string itemId);

        Task<HttpResponseMessage> Create(Collection collection);

        Task<HttpResponseMessage> Update(Collection collection);

        Task<HttpResponseMessage> Delete(string id);

        Task<HttpResponseMessage> CreateItem(string collectionId, CollectionItem item);

        Task<HttpResponseMessage> UpdateItem(string collectionId, CollectionItem item);

        Task<HttpResponseMessage> DeleteItem(string collectionId, string content);
    }
}
