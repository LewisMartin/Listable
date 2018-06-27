using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Listable.MVCWebApp.Models;
using Microsoft.Extensions.Configuration;
using Listable.MVCWebApp.Services;
using System.Collections.Generic;
using Listable.CollectionMicroservice.DTO;
using Listable.MVCWebApp.ViewModels.Collections;
using Newtonsoft.Json;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace Listable.MVCWebApp.Controllers
{
    [Authorize]
    public class CollectionsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IBlobService _blobService;
        private readonly ICollectionsService _collectionsService;

        public CollectionsController(IConfiguration configuration, IBlobService blobService, ICollectionsService collectionsService)
        {
            _configuration = configuration;
            _blobService = blobService;
            _collectionsService = collectionsService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Overview");
        }

        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            var response = await _collectionsService.RetrieveAll(GetUserUniqueName());
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Error", "Home");

            var collections = JsonConvert.DeserializeObject<List<Collection>>(await response.Content.ReadAsStringAsync());

            var userCollections = new List<CollectionOverview>();
            foreach (var collection in collections)
            {
                userCollections.Add(new CollectionOverview() { CollectionId = collection.Id, CollectionName = collection.Name });
            }

            return View(new OverviewViewModel()
            {
                collections = userCollections
            });
        }

        [HttpGet]
        public async Task<IActionResult> Collection(string collectionId)
        {
            var response = await _collectionsService.Retrieve(collectionId);
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Error", "Home");

            var collection = JsonConvert.DeserializeObject<Collection>(await response.Content.ReadAsStringAsync());

            if (collection.DisplayFormat == CollectionDisplayFormat.Grid)
                return RedirectToAction("CollectionGrid", new { Id = collectionId });

            var collectionItemNames = new List<Tuple<string, string>>();
            foreach (var item in collection.CollectionItems)
            {
                collectionItemNames.Add(new Tuple<string, string>(item.Id.ToString(), item.Name));
            }

            return View(new CollectionViewModel()
            {
                CollectionId = collectionId,
                CollectionName = collection.Name,
                CollectionItems = collectionItemNames
            });
        }

        [HttpGet]
        public async Task<IActionResult> CollectionGrid(string Id)
        {
            var response = await _collectionsService.Retrieve(Id);
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Error", "Home");

            var collection = JsonConvert.DeserializeObject<Collection>(await response.Content.ReadAsStringAsync());

            string imgIds = "";
            foreach (var item in collection.CollectionItems)
            {
                if (item.ImageId != null && item.ImageId != "")
                {
                    if (imgIds != "")
                        imgIds += "&";

                    imgIds += ("ids=" + item.ImageId);
                }
            }

            response = await _blobService.ImageRetrieveThumbs(imgIds);
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Error", "Home");

            var thumbnailMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(await response.Content.ReadAsStringAsync());

            var collectionItems = new List<CollectionGridItem>();

            foreach (var item in collection.CollectionItems)
            {
                collectionItems.Add(new CollectionGridItem() {
                    ItemId = item.Id.ToString(),
                    ItemName = item.Name,
                    ItemThumbUri = thumbnailMap.ContainsKey(item.ImageId) ? thumbnailMap[item.ImageId] : "" });
            }

            return View(new CollectionGridViewModel() {
                CollectionId = collection.Id,
                CollectionName = collection.Name,
                CollectionItems = collectionItems
            });
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
                    ImageEnabled = viewModel.IsImageEnabled,
                    DisplayFormat = viewModel.IsImageEnabled ? (viewModel.GridDisplay == true ? CollectionDisplayFormat.Grid : CollectionDisplayFormat.List) : CollectionDisplayFormat.List,
                    CollectionItems = new List<CollectionItem>()
                };

                if (!_collectionsService.Create(collection).Result.IsSuccessStatusCode)
                    return RedirectToAction("Error", "Home");

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
            var response = await _collectionsService.RetrieveAll(GetUserUniqueName());
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Error", "Home");

            var collections = JsonConvert.DeserializeObject<List<Collection>>(await response.Content.ReadAsStringAsync());

            var selectItems = new List<SelectListItem>();
            foreach (var collection in collections)
            {
                selectItems.Add(new SelectListItem()
                {
                    Value = collection.Id,
                    Text = collection.Name
                });
            }

            return View(new DeleteCollectionViewModel()
            {
                Collections = selectItems
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCollection(DeleteCollectionViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var response = await _collectionsService.Retrieve(viewModel.SelectedCollection);
                if (!response.IsSuccessStatusCode)
                    return RedirectToAction("Error", "Home");

                var collection = JsonConvert.DeserializeObject<Collection>(await response.Content.ReadAsStringAsync());

                if (collection.ImageEnabled)
                {
                    foreach (var item in collection.CollectionItems)
                    {
                        if (item.ImageId != null && item.ImageId != "")
                        {
                            if(!_blobService.ImageDelete(item.ImageId).Result.IsSuccessStatusCode)
                                return RedirectToAction("Error", "Home");
                        }
                    }
                }

                if(!_collectionsService.Delete(viewModel.SelectedCollection).Result.IsSuccessStatusCode)
                    return RedirectToAction("Error", "Home");

                return RedirectToAction("Overview");
            }
            else
            {
                return View();
            }
        }

        private string GetUserUniqueName()
        {
            string unique_name = "";

            foreach (var identity in User.Identities)
            {
                unique_name = identity.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").FirstOrDefault().Value;
            }

            return Regex.Replace(unique_name, "#", "-");
        }
    }
}