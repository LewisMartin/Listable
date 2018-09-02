using Listable.CollectionMicroservice.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Listable.MVCWebApp.Services
{
    public enum CollectionsApiAction
    {
        Retrieve,
        RetrieveAll,
        Create,
        CreateItem,
        Update,
        UpdateItem,
        Delete,
        DeleteItem
    }

    public class CollectionsService : BackendService<CollectionsApiAction>, ICollectionsService
    {
        public CollectionsService(IConfiguration configuration, IDistributedCache cache, IHttpContextAccessor accessor) : base(configuration, cache, accessor)
        {
        }

        public async Task<HttpResponseMessage> Retrieve(string collectionId)
        {
            return await APIRequest(CollectionsApiAction.Retrieve, "?collectionId=" + collectionId);
        }

        public async Task<HttpResponseMessage> RetrieveAll(string userId)
        {
            return await APIRequest(CollectionsApiAction.RetrieveAll, "?userId=" + userId);
        }

        public async Task<HttpResponseMessage> Create(Collection collection)
        {
            return await APIRequest(CollectionsApiAction.Create, "", new StringContent(JsonConvert.SerializeObject(collection).ToString(), Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> Update(Collection collection)
        {
            return await APIRequest(CollectionsApiAction.Update, ("?id=" + collection.Id), new StringContent(JsonConvert.SerializeObject(collection).ToString(), Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> Delete(string id)
        {
            return await APIRequest(CollectionsApiAction.Delete, "?id=" + id);
        }

        public async Task<HttpResponseMessage> CreateItem(string collectionId, CollectionItem item)
        {
            return await APIRequest(CollectionsApiAction.CreateItem, ("?collectionId=" + collectionId), new StringContent(JsonConvert.SerializeObject(item).ToString(), Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> UpdateItem(string collectionId, CollectionItem item)
        {
            return await APIRequest(CollectionsApiAction.UpdateItem, ("?collectionId=" + collectionId), new StringContent(JsonConvert.SerializeObject(item).ToString(), Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> DeleteItem(string collectionId, string content)
        {
            return await APIRequest(CollectionsApiAction.DeleteItem, ("?collectionId=" + collectionId), new StringContent(content, Encoding.UTF8, "application/json"));
        }

        protected override async Task<HttpResponseMessage> APIRequest(CollectionsApiAction action, string uriParams = "", HttpContent content = null)
        {
            var req = FormAPIRequestMessage(action, uriParams);

            if (action == CollectionsApiAction.Create || action == CollectionsApiAction.Update || action == CollectionsApiAction.UpdateItem || action == CollectionsApiAction.CreateItem || action == CollectionsApiAction.DeleteItem)
                req.Content = content;

            string accessToken = await GetAccessTokenAsync(ListableAPI.CollectionAPI);
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            return await Client.SendAsync(req);
        }

        protected override HttpRequestMessage FormAPIRequestMessage(CollectionsApiAction action, string uriParams)
        {
            switch (action)
            {
                case CollectionsApiAction.Retrieve:
                    return new HttpRequestMessage(HttpMethod.Get, (_configuration["CollectionAPI:APIEndpoint"] + "/retrieve" + uriParams));
                case CollectionsApiAction.RetrieveAll:
                    return new HttpRequestMessage(HttpMethod.Get, (_configuration["CollectionAPI:APIEndpoint"] + "/retrieveall" + uriParams));
                case CollectionsApiAction.Create:
                    return new HttpRequestMessage(HttpMethod.Post, (_configuration["CollectionAPI:APIEndpoint"] + "/create" + uriParams));
                case CollectionsApiAction.CreateItem:
                    return new HttpRequestMessage(HttpMethod.Post, (_configuration["CollectionAPI:APIEndpoint"] + "/createitem" + uriParams));
                case CollectionsApiAction.Update:
                    return new HttpRequestMessage(HttpMethod.Put, (_configuration["CollectionAPI:APIEndpoint"] + "/update" + uriParams));
                case CollectionsApiAction.UpdateItem:
                    return new HttpRequestMessage(HttpMethod.Put, (_configuration["CollectionAPI:APIEndpoint"] + "/updateitem" + uriParams));
                case CollectionsApiAction.Delete:
                    return new HttpRequestMessage(HttpMethod.Delete, (_configuration["CollectionAPI:APIEndpoint"] + "/delete" + uriParams));
                case CollectionsApiAction.DeleteItem:
                    return new HttpRequestMessage(HttpMethod.Post, (_configuration["CollectionAPI:APIEndpoint"] + "/deleteitem" + uriParams));
                default:
                    return new HttpRequestMessage();
            }
        }
    }
}
