using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Listable.MVCWebApp.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Listable.MVCWebApp.Services;
using System.Collections.Generic;
using Listable.CollectionMicroservice.DTO;
using Listable.MVCWebApp.ViewModels.Collections;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace Listable.MVCWebApp.Controllers
{
    [Authorize]
    public class CollectionsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ImageManipulation _imageManipulation;
        private readonly BlobService _blobService;
        private readonly CollectionsService _collectionsService;

        public CollectionsController(IConfiguration configuration, ImageManipulation imageManipulation, BlobService blobService, CollectionsService collectionsService)
        {
            _configuration = configuration;
            _imageManipulation = imageManipulation;
            _blobService = blobService;
            _collectionsService = collectionsService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Overview");
        }

        public async Task<IActionResult> Overview()
        {
            HttpResponseMessage res = await _collectionsService.APIRequest(CollectionsApiAction.RetrieveAll, ("?userId=" + GetUserUniqueName()));
            var collections = JsonConvert.DeserializeObject<List<Collection>>(await res.Content.ReadAsStringAsync());

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
            HttpResponseMessage res = await _collectionsService.APIRequest(CollectionsApiAction.Retrieve, ("?collectionId=" + collectionId));
            var collection = JsonConvert.DeserializeObject<Collection>(await res.Content.ReadAsStringAsync());

            if (collection.DisplayFormat == CollectionDisplayFormat.Grid)
                return RedirectToAction("CollectionGrid", new { Id = collectionId });

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
        public async Task<IActionResult> CollectionGrid(string Id)
        {
            HttpResponseMessage res = await _collectionsService.APIRequest(CollectionsApiAction.Retrieve, ("?collectionId=" + Id));
            var collection = JsonConvert.DeserializeObject<Collection>(await res.Content.ReadAsStringAsync());

            string uriParams = "?";

            foreach (var item in collection.CollectionItems)
            {
                if (item.ImageId != null && item.ImageId != "")
                {
                    if (uriParams != "?")
                        uriParams += "&";

                    uriParams += ("ids=" + item.ImageId);
                }
            }

            res = await _blobService.APIRequest(BlobApiAction.ImageRetrieveThumbs, uriParams);
            var thumbnailMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(await res.Content.ReadAsStringAsync());

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

                HttpResponseMessage res = await _collectionsService.APIRequest(CollectionsApiAction.Create, "", new StringContent(JsonConvert.SerializeObject(collection).ToString(), Encoding.UTF8, "application/json"));
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
            HttpResponseMessage res = await _collectionsService.APIRequest(CollectionsApiAction.RetrieveAll, ("?userId=" + GetUserUniqueName()));
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
                // first, delete any images associated with the collection
                HttpResponseMessage res = await _collectionsService.APIRequest(CollectionsApiAction.Retrieve, ("?collectionId=" + viewModel.SelectedCollection));
                var collection = JsonConvert.DeserializeObject<Collection>(await res.Content.ReadAsStringAsync());

                if (collection.ImageEnabled)
                {
                    foreach (var item in collection.CollectionItems)
                    {
                        if(item.ImageId != null && item.ImageId != "")
                        {
                            HttpResponseMessage blobRes = await _blobService.APIRequest(BlobApiAction.ImageDelete, "?id=" + item.ImageId);
                        }
                    }
                }

                res = await _collectionsService.APIRequest(CollectionsApiAction.Delete, ("?id=" + viewModel.SelectedCollection));
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
            HttpResponseMessage res = await _collectionsService.APIRequest(CollectionsApiAction.Retrieve, ("?collectionId=" + collectionId));
            var collection = JsonConvert.DeserializeObject<Collection>(await res.Content.ReadAsStringAsync());

            var item = collection.CollectionItems.Where(i => i.Id == new Guid(itemId)).FirstOrDefault();

            var url = "";
            if (collection.ImageEnabled && item.ImageId != null && item.ImageId != "")
            {
                HttpResponseMessage blobRes = await _blobService.APIRequest(BlobApiAction.ImageRetrieveUrl, "?id=" + item.ImageId);
                url = await blobRes.Content.ReadAsStringAsync();
            }

            ViewItemViewModel viewModel = new ViewItemViewModel()
            {
                CollectionId = collection.Id,
                CollectionName = collection.Name,
                Name = item.Name,
                Description = item.Description,
                ImageEnabled = collection.ImageEnabled,
                ImageUrl = url
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CreateItem(string collectionId)
        {
            HttpResponseMessage res = await _collectionsService.APIRequest(CollectionsApiAction.Retrieve, ("?collectionId=" + collectionId));
            var collection = JsonConvert.DeserializeObject<Collection>(await res.Content.ReadAsStringAsync());

            CreateItemViewModel viewModel = new CreateItemViewModel()
            {
                CollectionId = collection.Id,
                CollectionName = collection.Name,
                ImageEnabled = collection.ImageEnabled
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateItem(CreateItemViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string imgId = "";

                if (viewModel.ImageFile != null && viewModel.ImageFile.Length > 0)
                {
                    string ImageContentType = viewModel.ImageFile.ContentType;
                    var fileName = ContentDispositionHeaderValue.Parse(viewModel.ImageFile.ContentDisposition).FileName;

                    if (viewModel.ImageFile.Length > 100000)
                    {
                        viewModel.ImageFile = _imageManipulation.LoadFile(viewModel.ImageFile).Resize(600).Retrieve();
                        ImageContentType = "image/jpg";
                    }

                    var content = new MultipartFormDataContent
                    {
                        {
                            new StreamContent(viewModel.ImageFile.OpenReadStream())
                            {
                                Headers =
                                {
                                    ContentLength = viewModel.ImageFile.Length,
                                    ContentType = new MediaTypeHeaderValue(ImageContentType)
                                }
                            },
                            "image",
                            fileName
                        }
                    };

                    HttpResponseMessage blobRes = await _blobService.APIRequest(BlobApiAction.ImageUpload, "", content);
                    imgId = await blobRes.Content.ReadAsStringAsync();
                }

                CollectionItem item = new CollectionItem()
                {
                    Id = Guid.NewGuid(),
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    ImageId = imgId
                };

                HttpResponseMessage res = await _collectionsService.APIRequest(CollectionsApiAction.CreateItem, ("?collectionId=" + viewModel.CollectionId), new StringContent(JsonConvert.SerializeObject(item).ToString(), Encoding.UTF8, "application/json"));
                return RedirectToAction("Collection", new { collectionId = viewModel.CollectionId });
            }
            else
            {
                return View(viewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteItem(string collectionId)
        {
            HttpResponseMessage res = await _collectionsService.APIRequest(CollectionsApiAction.Retrieve, ("?collectionId=" + collectionId));
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
            HttpResponseMessage res = await _collectionsService.APIRequest(CollectionsApiAction.Retrieve, ("?collectionId=" + viewModel.CollectionId));
            var collection = JsonConvert.DeserializeObject<Collection>(await res.Content.ReadAsStringAsync());

            var itemIds = new List<string>();
            var imageIds = new List<string>();
            foreach (var item in viewModel.DeleteItemOptions)
            {
                if (item.IsOptionSelected)
                {
                    itemIds.Add(item.ItemId);
                    imageIds.Add(collection.CollectionItems.Where(i => i.Id.ToString() == item.ItemId).FirstOrDefault().ImageId);
                }
            }

            if (collection.ImageEnabled)
            {
                // delete media
                foreach (var imageId in imageIds)
                {
                    HttpResponseMessage blobRes = await _blobService.APIRequest(BlobApiAction.ImageDelete, "?id=" + imageId);
                }
            }

            // delete items
            var content = JsonConvert.SerializeObject(itemIds);
            res = await _collectionsService.APIRequest(CollectionsApiAction.DeleteItem, ("?collectionId=" + viewModel.CollectionId), new StringContent(content, Encoding.UTF8, "application/json"));
            var success = await res.Content.ReadAsStringAsync();

            return RedirectToAction("Collection", new { collectionId = viewModel.CollectionId });
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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