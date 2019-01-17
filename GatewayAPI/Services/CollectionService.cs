using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Listable.CollectionMicroservice.DTO;

namespace GatewayAPI.Services
{
    public enum CollectionApiAction
    {
        CheckPermissions,
        Retrieve,
        RetrieveAll,
        Query,
        RetrieveItem,
        Create,
        CreateItem,
        Update,
        UpdateItem,
        Delete,
        DeleteItem
    }

    public class CollectionService : BackendService<CollectionApiAction>, ICollectionsService
    {
        public CollectionService(IConfiguration configuration, IDistributedCache cache, IHttpContextAccessor accessor) : base(configuration, cache, accessor) { }

        public async Task<HttpResponseMessage> CheckPermissions(int userId, string collectionId, PermissionType permType)
        {
            return await APIRequest(CollectionApiAction.CheckPermissions, "?userId=" + userId + "&collectionId=" + collectionId + "&permType=" + permType);
        }

        public async Task<HttpResponseMessage> Retrieve(string collectionId)
        {
            return await APIRequest(CollectionApiAction.Retrieve, "?collectionId=" + collectionId);
        }

        public async Task<HttpResponseMessage> RetrieveAll(int userId)
        {
            return await APIRequest(CollectionApiAction.RetrieveAll, "?userId=" + userId);
        }

        public async Task<HttpResponseMessage> Query(CollectionQuery query)
        {
            return await APIRequest(CollectionApiAction.Query, "", new StringContent(JsonConvert.SerializeObject(query).ToString(), Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> Create(Collection collection)
        {
            return await APIRequest(CollectionApiAction.Create, "", new StringContent(JsonConvert.SerializeObject(collection).ToString(), Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> Update(Collection collection)
        {
            return await APIRequest(CollectionApiAction.Update, ("?id=" + collection.Id), new StringContent(JsonConvert.SerializeObject(collection).ToString(), Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> Delete(string id)
        {
            return await APIRequest(CollectionApiAction.Delete, "?id=" + id);
        }

        public async Task<HttpResponseMessage> RetrieveItem(string collectionId, string itemId)
        {
            return await APIRequest(CollectionApiAction.RetrieveItem, "?collectionId=" + collectionId + "&itemId=" + itemId);
        }

        public async Task<HttpResponseMessage> CreateItem(string collectionId, CollectionItem item)
        {
            return await APIRequest(CollectionApiAction.CreateItem, ("?collectionId=" + collectionId), new StringContent(JsonConvert.SerializeObject(item).ToString(), Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> UpdateItem(string collectionId, CollectionItem item)
        {
            return await APIRequest(CollectionApiAction.UpdateItem, ("?collectionId=" + collectionId), new StringContent(JsonConvert.SerializeObject(item).ToString(), Encoding.UTF8, "application/json"));
        }

        public async Task<HttpResponseMessage> DeleteItem(string collectionId, string content)
        {
            return await APIRequest(CollectionApiAction.DeleteItem, ("?collectionId=" + collectionId), new StringContent(content, Encoding.UTF8, "application/json"));
        }

        protected override async Task<HttpResponseMessage> APIRequest(CollectionApiAction action, string uriParams = "", HttpContent content = null)
        {
            var req = CreateAPIRequestMessage(action, uriParams);

            if (action == CollectionApiAction.Query || action == CollectionApiAction.Create || action == CollectionApiAction.Update || action == CollectionApiAction.UpdateItem || action == CollectionApiAction.CreateItem || action == CollectionApiAction.DeleteItem)
                req.Content = content;

            string accessToken = await GetAccessTokenAsync(BackendAPI.CollectionAPI);
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            return await Client.SendAsync(req);
        }

        protected override HttpRequestMessage CreateAPIRequestMessage(CollectionApiAction action, string uriParams)
        {
            switch (action)
            {
                case CollectionApiAction.CheckPermissions:
                    return new HttpRequestMessage(HttpMethod.Get, _configuration["CollectionAPI:APIEndpoint"] + "/checkpermissions" + uriParams);
                case CollectionApiAction.Retrieve:
                    return new HttpRequestMessage(HttpMethod.Get, (_configuration["CollectionAPI:APIEndpoint"] + "/retrieve" + uriParams));
                case CollectionApiAction.RetrieveAll:
                    return new HttpRequestMessage(HttpMethod.Get, (_configuration["CollectionAPI:APIEndpoint"] + "/retrieveall" + uriParams));
                case CollectionApiAction.Query:
                    return new HttpRequestMessage(HttpMethod.Post, (_configuration["CollectionAPI:APIEndpoint"] + "/query" + uriParams));
                case CollectionApiAction.RetrieveItem:
                    return new HttpRequestMessage(HttpMethod.Get, (_configuration["CollectionAPI:APIEndpoint"] + "/retrieveitem" + uriParams));
                case CollectionApiAction.Create:
                    return new HttpRequestMessage(HttpMethod.Post, (_configuration["CollectionAPI:APIEndpoint"] + "/create" + uriParams));
                case CollectionApiAction.CreateItem:
                    return new HttpRequestMessage(HttpMethod.Post, (_configuration["CollectionAPI:APIEndpoint"] + "/createitem" + uriParams));
                case CollectionApiAction.Update:
                    return new HttpRequestMessage(HttpMethod.Put, (_configuration["CollectionAPI:APIEndpoint"] + "/update" + uriParams));
                case CollectionApiAction.UpdateItem:
                    return new HttpRequestMessage(HttpMethod.Put, (_configuration["CollectionAPI:APIEndpoint"] + "/updateitem" + uriParams));
                case CollectionApiAction.Delete:
                    return new HttpRequestMessage(HttpMethod.Delete, (_configuration["CollectionAPI:APIEndpoint"] + "/delete" + uriParams));
                case CollectionApiAction.DeleteItem:
                    return new HttpRequestMessage(HttpMethod.Post, (_configuration["CollectionAPI:APIEndpoint"] + "/deleteitem" + uriParams));
                default:
                    return new HttpRequestMessage();
            }
        }
    }
}
