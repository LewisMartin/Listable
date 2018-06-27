using Listable.CollectionMicroservice.DTO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Listable.MVCWebApp.Services
{
    public interface ICollectionsService
    {
        Task<HttpResponseMessage> Retrieve(string uriParams);

        Task<HttpResponseMessage> RetrieveAll(string uriParams);

        Task<HttpResponseMessage> Create(Collection collection);

        Task<HttpResponseMessage> Delete(string uriParams);

        Task<HttpResponseMessage> CreateItem(string collectionId, CollectionItem item);

        Task<HttpResponseMessage> DeleteItem(string collectionId, string content);
    }
}