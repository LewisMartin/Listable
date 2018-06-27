using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Listable.MVCWebApp.Services
{
    public enum ListableAPI
    {
        CollectionAPI,
        BlobAPI
    }

    public abstract class BackendService<T>
    {
        protected static readonly HttpClient Client = new HttpClient();
        protected readonly IConfiguration _configuration;
        protected readonly IDistributedCache _cache;
        protected readonly ClaimsPrincipal _user;

        public BackendService(IConfiguration configuration, IDistributedCache cache, IHttpContextAccessor accessor)
        {
            _configuration = configuration;
            _cache = cache;
            _user = accessor.HttpContext.User;
        }

        protected async Task<string> GetAccessTokenAsync(ListableAPI api)
        {
            string resource = GetAPIResource(api);

            if (resource != null)
            {
                string authority = _configuration["AzureAd:Authority"];

                string userId = _user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
                var cache = new AdalDistributedTokenCache(_cache, userId);

                var authContext = new AuthenticationContext(authority, cache);

                string clientId = _configuration["AzureAd:ClientId"];
                string clientSecret = _configuration["AzureAd:ClientSecret"];
                var credential = new ClientCredential(clientId, clientSecret);

                var result = await authContext.AcquireTokenSilentAsync(resource, credential, new UserIdentifier(userId, UserIdentifierType.UniqueId));

                return result.AccessToken;
            }
            else
            {
                throw new NullReferenceException();
            }
        }

        protected string GetAPIResource(ListableAPI api)
        {
            switch (api)
            {
                case ListableAPI.CollectionAPI:
                    return _configuration["CollectionAPI:Resource"];
                case ListableAPI.BlobAPI:
                    return _configuration["BlobServiceAPI:Resource"];
                default:
                    return null;
            }
        }

        protected abstract Task<HttpResponseMessage> APIRequest(T action, string uriParams = "", HttpContent content = null);

        protected abstract HttpRequestMessage FormAPIRequestMessage(T action, string uriParams);
    }
}
