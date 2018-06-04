﻿using System.Diagnostics;
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

        public IActionResult Index()
        {
            return RedirectToAction("Overview");
        }

        public async Task<IActionResult> Overview()
        {
            HttpResponseMessage res = await CollectionsAPIRequest(CollectionsApiMethod.RetrieveAll, "?userId=testuser");
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
        public IActionResult Collection(string collectionId)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateViewModel viewModel)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Delete()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Delete(DeleteViewModel viewModel)
        {
            return View();
        }

        public IActionResult ViewItem()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateItem()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateItem(CreateItemViewModel viewModel)
        {
            return View();
        }

        [HttpGet]
        public IActionResult DeleteItem()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteItem(DeleteItemViewModel viewModel)
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<HttpResponseMessage> CollectionsAPIRequest(CollectionsApiMethod method, string uriParams = "")
        {
            var req = FormRequestMessage(method, uriParams);


            string accessToken = await GetAccessTokenAsync();
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            return await Client.SendAsync(req);
        }

        private HttpRequestMessage FormRequestMessage(CollectionsApiMethod method, string uriParams)
        {
            switch (method)
            {
                case CollectionsApiMethod.Retrieve:
                    return new HttpRequestMessage(HttpMethod.Get, (_configuration["CollectionAPI:APIEndpoint"] + "/retrieve" + uriParams));
                case CollectionsApiMethod.RetrieveAll:
                    return new HttpRequestMessage(HttpMethod.Get, (_configuration["CollectionAPI:APIEndpoint"] + "/retrieveall" + uriParams));
                case CollectionsApiMethod.Create:
                    return new HttpRequestMessage(HttpMethod.Post, (_configuration["CollectionAPI:APIEndpoint"] + "/create" + uriParams));
                case CollectionsApiMethod.Update:
                    return new HttpRequestMessage(HttpMethod.Put, (_configuration["CollectionAPI:APIEndpoint"] + "/update" + uriParams));
                case CollectionsApiMethod.Delete:
                    return new HttpRequestMessage(HttpMethod.Delete, (_configuration["CollectionAPI:APIEndpoint"] + "/delete" + uriParams));
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