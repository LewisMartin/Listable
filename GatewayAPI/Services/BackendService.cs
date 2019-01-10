using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace GatewayAPI.Services
{
    public abstract class BackendService<T>
    {
        public enum BackendAPI
        {
            CollectionAPI,
            BlobAPI,
            UserAPI
        }

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

        protected async Task<string> GetAccessTokenAsync(BackendAPI api)
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

                ClaimsIdentity identity =_user.Identity as ClaimsIdentity;
                string userAccessToken = identity.BootstrapContext as string;
                string userName = (_user.FindFirst(ClaimTypes.Upn))?.Value;

                UserAssertion userAssertion = new UserAssertion(userAccessToken,
                                             "urn:ietf:params:oauth:grant-type:jwt-bearer",
                                                userName);

                AuthenticationResult result = null;
                try
                {
                    result = await authContext.AcquireTokenSilentAsync(resource, clientId);
                }
                catch (AdalException adalException)
                {
                    if (adalException.ErrorCode == AdalError.FailedToAcquireTokenSilently || adalException.ErrorCode == AdalError.UserInteractionRequired)
                    {
                        result = await authContext.AcquireTokenAsync(resource, credential, userAssertion);
                    }
                }

                return result.AccessToken;
            }
            else
            {
                throw new NullReferenceException();
            }
        }

        protected string GetAPIResource(BackendAPI api)
        {
            switch (api)
            {
                case BackendAPI.CollectionAPI:
                    return _configuration["CollectionAPI:Resource"];
                case BackendAPI.BlobAPI:
                    return _configuration["BlobServiceAPI:Resource"];
                case BackendAPI.UserAPI:
                    return _configuration["UserServiceAPI:Resource"];
                default:
                    return null;
            }
        }

        protected abstract Task<HttpResponseMessage> APIRequest(T action, string uriParams = "", HttpContent content = null);

        protected abstract HttpRequestMessage CreateAPIRequestMessage(T action, string uriParams);
    }
}
