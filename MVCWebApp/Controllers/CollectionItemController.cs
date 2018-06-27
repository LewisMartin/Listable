using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Listable.MVCWebApp.Services;
using System.Collections.Generic;
using Listable.CollectionMicroservice.DTO;
using Listable.MVCWebApp.ViewModels.Collections;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http.Headers;

namespace Listable.MVCWebApp.Controllers
{
    public class CollectionItemController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IImageManipulation _imageManipulation;
        private readonly IBlobService _blobService;
        private readonly ICollectionsService _collectionsService;

        public CollectionItemController(IConfiguration configuration, IImageManipulation imageManipulation, IBlobService blobService, ICollectionsService collectionsService)
        {
            _configuration = configuration;
            _imageManipulation = imageManipulation;
            _blobService = blobService;
            _collectionsService = collectionsService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Overview", "Collections");
        }

        [HttpGet]
        public async Task<IActionResult> ViewItem(string collectionId, string itemId)
        {
            var response = await _collectionsService.Retrieve(collectionId);
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Error", "Home");

            var collection = JsonConvert.DeserializeObject<Collection>(await response.Content.ReadAsStringAsync());

            var item = collection.CollectionItems.Where(i => i.Id == new Guid(itemId)).FirstOrDefault();

            if (item == null)
                return RedirectToAction("Error", "Home");

            var url = "";
            if (collection.ImageEnabled && item.ImageId != null && item.ImageId != "")
            {
                response = await _blobService.ImageRetrieveUrl(item.ImageId);
                if (!response.IsSuccessStatusCode)
                    return RedirectToAction("Error", "Home");

                url = JsonConvert.DeserializeObject<string>(await response.Content.ReadAsStringAsync());
            }

            return View(new ViewItemViewModel()
            {
                CollectionId = collection.Id,
                CollectionName = collection.Name,
                Name = item.Name,
                Description = item.Description,
                ImageEnabled = collection.ImageEnabled,
                ImageUrl = url
            });
        }

        [HttpGet]
        public async Task<IActionResult> CreateItem(string collectionId)
        {
            var response = await _collectionsService.Retrieve(collectionId);
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Error", "Home");

            var collection = JsonConvert.DeserializeObject<Collection>(await response.Content.ReadAsStringAsync());

            return View(new CreateItemViewModel()
            {
                CollectionId = collection.Id,
                CollectionName = collection.Name,
                ImageEnabled = collection.ImageEnabled
            });
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

                    var response = await _blobService.ImageUpload(content);
                    if (!response.IsSuccessStatusCode)
                        return RedirectToAction("Error", "Home");

                    imgId = JsonConvert.DeserializeObject<string>(await response.Content.ReadAsStringAsync());
                }

                CollectionItem item = new CollectionItem()
                {
                    Id = Guid.NewGuid(),
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    ImageId = imgId
                };

                if (!_collectionsService.CreateItem(viewModel.CollectionId, item).Result.IsSuccessStatusCode)
                    return RedirectToAction("Error", "Home");

                return RedirectToAction("Collection", "Collections", new { collectionId = viewModel.CollectionId });
            }
            else
            {
                return View(viewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteItem(string collectionId)
        {
            var response = await _collectionsService.Retrieve(collectionId);
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Error", "Home");

            var collection = JsonConvert.DeserializeObject<Collection>(await response.Content.ReadAsStringAsync());

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

            return View(new DeleteItemViewModel()
            {
                CollectionId = collection.Id,
                CollectionName = collection.Name,
                DeleteItemOptions = deleteItemOptions
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(DeleteItemViewModel viewModel)
        {
            var response = await _collectionsService.Retrieve(viewModel.CollectionId);
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Error", "Home");

            var collection = JsonConvert.DeserializeObject<Collection>(await response.Content.ReadAsStringAsync());

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
                foreach (var imageId in imageIds)                   // delete media
                {
                    if (!_blobService.ImageDelete(imageId).Result.IsSuccessStatusCode)
                        return RedirectToAction("Error", "Home");
                }
            }

            var content = JsonConvert.SerializeObject(itemIds);     // delete items

            if (!_collectionsService.DeleteItem(viewModel.CollectionId, content).Result.IsSuccessStatusCode)
                return RedirectToAction("Error", "Home");

            return RedirectToAction("Collection", "Collections", new { collectionId = viewModel.CollectionId });
        }
    }
}