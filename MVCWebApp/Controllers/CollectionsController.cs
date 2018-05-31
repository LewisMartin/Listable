using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Listable.MVCWebApp.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Microsoft.Extensions.Caching.Distributed;
using Listable.MVCWebApp.Services;
using System.Collections.Generic;
using Listable.CollectionMicroservice.DTO;

namespace Listable.MVCWebApp.Controllers
{
    enum CollectionsApiMethod
    {
        Retrieve,
        RetrieveAll,
        Create,
        Update,
        Delete
    }

    [Authorize]
    public class CollectionsController : Controller
    {
        private static readonly HttpClient Client = new HttpClient();
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;

        public CollectionsController(IConfiguration configuration, IDistributedCache cache)
        {
            _configuration = configuration;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            HttpResponseMessage res = await CollectionsAPIRequest(CollectionsApiMethod.RetrieveAll);

            ViewBag.CollectionsResponse = await res.Content.ReadAsStringAsync();

            return View();
        }

        public IActionResult Collections()
        {
            return View();
        }

        public IActionResult Collection()
        {
            return View();
        }

        public IActionResult CollectionItem()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<HttpResponseMessage> CollectionsAPIRequest(CollectionsApiMethod method)
        {
            var req = FormRequestMessage(method);


            string accessToken = await GetAccessTokenAsync();
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            return await Client.SendAsync(req);
        }

        private HttpRequestMessage FormRequestMessage(CollectionsApiMethod method)
        {
            switch (method)
            {
                case CollectionsApiMethod.Retrieve:
                    return new HttpRequestMessage(HttpMethod.Get, (_configuration["CollectionAPI:APIEndpoint"] + "/retrieve"));
                case CollectionsApiMethod.RetrieveAll:
                    return new HttpRequestMessage(HttpMethod.Get, (_configuration["CollectionAPI:APIEndpoint"] + "/retrieveall"));
                case CollectionsApiMethod.Create:
                    return new HttpRequestMessage(HttpMethod.Post, (_configuration["CollectionAPI:APIEndpoint"] + "/create"));
                case CollectionsApiMethod.Update:
                    return new HttpRequestMessage(HttpMethod.Put, (_configuration["CollectionAPI:APIEndpoint"] + "/update"));
                case CollectionsApiMethod.Delete:
                    return new HttpRequestMessage(HttpMethod.Delete, (_configuration["CollectionAPI:APIEndpoint"] + "/delete"));
                default:
                    return new HttpRequestMessage();
            }
        }

        private async Task<string> GetAccessTokenAsync()
        {
            string authority = _configuration["AzureAd:Authority"];

            string userId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            var cache = new AdalDistributedTokenCache(_cache, userId);

            var authContext = new AuthenticationContext(authority, cache);

            string clientId = _configuration["AzureAd:ClientId"];
            string clientSecret = _configuration["AzureAd:ClientSecret"];
            var credential = new ClientCredential(clientId, clientSecret);

            var result = await authContext.AcquireTokenSilentAsync(_configuration["AzureAd:Resource"], credential, new UserIdentifier(userId, UserIdentifierType.UniqueId));

            return result.AccessToken;
        }
    }
}