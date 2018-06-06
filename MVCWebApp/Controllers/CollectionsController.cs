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
using Listable.MVCWebApp.ViewModels.Collections;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Listable.MVCWebApp.Controllers
{
    enum ListableAPI
    {
        CollectionAPI,
        BlobAPI
    }

    enum CollectionsApiAction
    {
        Retrieve,
        RetrieveAll,
        Create,
        CreateItem,
        Update,
        Delete,
        DeleteItem
    }

    enum BlobApiAction
    {
        ImageUpload,
        ImageDelete,
        ImageRetrieve
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

        public IActionResult Index()
        {
            return RedirectToAction("Overview");
        }

        public async Task<IActionResult> Overview()
        {
            HttpResponseMessage res = await CollectionsAPIRequest(CollectionsApiAction.RetrieveAll, ("?userId=" + GetUserUniqueName()));
            var collections = JsonConvert.DeserializeObject<List<Collection>>(await res.Content.ReadAsStringAsync());

            var userCollections = new List<Tuple<string, string>>();
            foreach (var collection in collections)
            {
                userCollections.Add(new Tuple<string, string>(collection.Id, collection.Name));
            }

            OverviewViewModel viewModel = new OverviewViewModel()
            {
                collections = userCollections
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Collection(string collectionId)
        {
            HttpResponseMessage res = await CollectionsAPIRequest(CollectionsApiAction.Retrieve, ("?collectionId=" + collectionId));
            var collection = JsonConvert.DeserializeObject<Collection>(await res.Content.ReadAsStringAsync());

            var collectionItemNames = new List<Tuple<string, string>>();
            foreach (var item in collection.CollectionItems)
            {
                collectionItemNames.Add(new Tuple<string, string>(item.Id.ToString(), item.Name));
            }

            CollectionViewModel viewModel = new CollectionViewModel()
            {
                CollectionId = collectionId,
                CollectionName = collection.Name,
                CollectionItems = collectionItemNames
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult CreateCollection()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCollection(CreateCollectionViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Collection collection = new Collection()
                {
                    Name = viewModel.Name,
                    Owner = GetUserUniqueName(),
                    CollectionItems = new List<CollectionItem>()
                };

                HttpResponseMessage res = await CollectionsAPIRequest(CollectionsApiAction.Create, "", JsonConvert.SerializeObject(collection).ToString());
                return RedirectToAction("Overview");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCollection()
        {
            // pull this into own method
            HttpResponseMessage res = await CollectionsAPIRequest(CollectionsApiAction.RetrieveAll, ("?userId=" + GetUserUniqueName()));
            var collections = JsonConvert.DeserializeObject<List<Collection>>(await res.Content.ReadAsStringAsync());

            var selectItems = new List<SelectListItem>();

            foreach (var collection in collections)
            {
                selectItems.Add(new SelectListItem()
                {
                    Value = collection.Id,
                    Text = collection.Name
                });
            }

            DeleteCollectionViewModel viewModel = new DeleteCollectionViewModel()
            {
                Collections = selectItems
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCollection(DeleteCollectionViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage res = await CollectionsAPIRequest(CollectionsApiAction.Delete, ("?id=" + viewModel.SelectedCollection));
                var success = await res.Content.ReadAsStringAsync();

                return RedirectToAction("Overview");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewItem(string collectionId, string itemId)
        {
            HttpResponseMessage res = await CollectionsAPIRequest(CollectionsApiAction.Retrieve, ("?collectionId=" + collectionId));
            var collection = JsonConvert.DeserializeObject<Collection>(await res.Content.ReadAsStringAsync());

            var item = collection.CollectionItems.Where(i => i.Id == new Guid(itemId)).FirstOrDefault();

            // test connectivity to blobservice API
            HttpResponseMessage blobRes = await BlobAPIRequest(BlobApiAction.ImageRetrieve, "?id=5");
            var url = await blobRes.Content.ReadAsStringAsync();

            ViewItemViewModel viewModel = new ViewItemViewModel()
            {
                CollectionId = collection.Id,
                CollectionName = collection.Name,
                Name = item.Name,
                Description = item.Description,
                ImageUrl = url
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CreateItem(string collectionId)
        {
            HttpResponseMessage res = await CollectionsAPIRequest(CollectionsApiAction.Retrieve, ("?collectionId=" + collectionId));
            var collection = JsonConvert.DeserializeObject<Collection>(await res.Content.ReadAsStringAsync());

            CreateItemViewModel viewModel = new CreateItemViewModel()
            {
                CollectionId = collection.Id,
                CollectionName = collection.Name,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateItem(CreateItemViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                CollectionItem item = new CollectionItem()
                {
                    Id = Guid.NewGuid(),
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                };

                HttpResponseMessage res = await CollectionsAPIRequest(CollectionsApiAction.CreateItem, ("?collectionId=" + viewModel.CollectionId), JsonConvert.SerializeObject(item).ToString());
                return RedirectToAction("Collection", new { collectionId = viewModel.CollectionId });
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteItem(string collectionId)
        {
            HttpResponseMessage res = await CollectionsAPIRequest(CollectionsApiAction.Retrieve, ("?collectionId=" + collectionId));
            var collection = JsonConvert.DeserializeObject<Collection>(await res.Content.ReadAsStringAsync());

            var deleteItemOptions = new List<DeleteItemOption>();

            foreach (var item in collection.CollectionItems)
            {
                deleteItemOptions.Add(new DeleteItemOption()
                {
                    IsOptionSelected = false,
                    ItemId = item.Id.ToString(),
                    ItemName = item.Name
                });
            }

            DeleteItemViewModel viewModel = new DeleteItemViewModel()
            {
                CollectionId = collection.Id,
                CollectionName = collection.Name,
                DeleteItemOptions = deleteItemOptions
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(DeleteItemViewModel viewModel)
        {
            var itemIds = new List<string>();
            foreach (var item in viewModel.DeleteItemOptions)
            {
                if (item.IsOptionSelected)
                    itemIds.Add(item.ItemId);
            }

            var content = JsonConvert.SerializeObject(itemIds);

            HttpResponseMessage res = await CollectionsAPIRequest(CollectionsApiAction.DeleteItem, ("?collectionId=" + viewModel.CollectionId), content);
            var success = await res.Content.ReadAsStringAsync();

            return RedirectToAction("Collection", new { collectionId = viewModel.CollectionId });
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<HttpResponseMessage> CollectionsAPIRequest(CollectionsApiAction method, string uriParams = "", string content = "")
        {
            var req = FormCollectionsAPIRequestMessage(method, uriParams);

            if (method == CollectionsApiAction.Create || method == CollectionsApiAction.CreateItem || method == CollectionsApiAction.DeleteItem)
                req.Content = new StringContent(content, Encoding.UTF8, "application/json");

            string accessToken = await GetAccessTokenAsync(ListableAPI.CollectionAPI);
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            return await Client.SendAsync(req);
        }

        private HttpRequestMessage FormCollectionsAPIRequestMessage(CollectionsApiAction method, string uriParams)
        {
            switch (method)
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

        private async Task<HttpResponseMessage> BlobAPIRequest(BlobApiAction action, string uriParams = "", string content = "")
        {
            var req = FormBlobAPIRequestMessage(action, uriParams);

            string accessToken = await GetAccessTokenAsync(ListableAPI.BlobAPI);
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            return await Client.SendAsync(req);
        }

        private HttpRequestMessage FormBlobAPIRequestMessage(BlobApiAction action, string uriParams)
        {
            switch (action)
            {
                case BlobApiAction.ImageUpload:
                    return new HttpRequestMessage(HttpMethod.Get, (_configuration["BlobServiceAPI:APIEndpoint"] + "/upload" + uriParams));
                case BlobApiAction.ImageRetrieve:
                    return new HttpRequestMessage(HttpMethod.Get, (_configuration["BlobServiceAPI:APIEndpoint"] + "/retrieve" + uriParams));
                case BlobApiAction.ImageDelete:
                    return new HttpRequestMessage(HttpMethod.Get, (_configuration["BlobServiceAPI:APIEndpoint"] + "/delete" + uriParams));
                default:
                    return new HttpRequestMessage();
            }
        }

        private async Task<string> GetAccessTokenAsync(ListableAPI api)
        {
            string resource = GetAPIResource(api);

            if (resource != null)
            {
                string authority = _configuration["AzureAd:Authority"];

                string userId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
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

        private string GetAPIResource(ListableAPI api)
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

        private string GetUserUniqueName()
        {
            string unique_name = "";

            foreach (var identity in User.Identities)
            {
                unique_name = identity.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").FirstOrDefault().Value;
            }

            return unique_name;
        }
    }
}