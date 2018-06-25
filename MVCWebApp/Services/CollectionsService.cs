using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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
        Delete,
        DeleteItem
    }

    public class CollectionsService : BackendService<CollectionsApiAction>
    {
        public CollectionsService(IConfiguration configuration, IDistributedCache cache, IHttpContextAccessor accessor) : base(configuration, cache, accessor)
        {
        }

        // old: req.Content = new StringContent(content, Encoding.UTF8, "application/json");
        public override async Task<HttpResponseMessage> APIRequest(CollectionsApiAction action, string uriParams = "", HttpContent content = null)
        {
            var req = FormAPIRequestMessage(action, uriParams);

            if (action == CollectionsApiAction.Create || action == CollectionsApiAction.CreateItem || action == CollectionsApiAction.DeleteItem)
                req.Content = content;

            string accessToken = await GetAccessTokenAsync(ListableAPI.CollectionAPI);
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            return await Client.SendAsync(req);
        }

        // To CollectionService, override of method on base class 'FormAPIRequestMessage'
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
