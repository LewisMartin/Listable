using Listable.CollectionMicroservice.DTO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Listable.MVCWebApp.Services
{
    public interface ICollectionsService
    {
        Task<HttpResponseMessage> Retrieve(string collectionId);

        Task<HttpResponseMessage> RetrieveAll(string userId);

        Task<HttpResponseMessage> Create(Collection collection);

        Task<HttpResponseMessage> Update(Collection collection);

        Task<HttpResponseMessage> Delete(string id);

        Task<HttpResponseMessage> CreateItem(string collectionId, CollectionItem item);

        Task<HttpResponseMessage> DeleteItem(string collectionId, string content);
    }
}