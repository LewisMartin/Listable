using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Listable.MVCWebApp.Services;
using System.Collections.Generic;
using Listable.CollectionMicroservice.DTO;
using Listable.MVCWebApp.ViewModels.Collections;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Listable.MVCWebApp.Controllers
{
    [Authorize]
    public class CollectionItemController : Controller
    {
        private readonly IImageManipulation _imageManipulation;
        private readonly IBlobService _blobService;
        private readonly ICollectionsService _collectionsService;

        public CollectionItemController(IImageManipulation imageManipulation, IBlobService blobService, ICollectionsService collectionsService)
        {
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
                Id = item.Id.ToString(),
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
                CollectionName = collection.Name,
                ItemDetails = new ItemEditor()
                {
                    CollectionId = collection.Id,
                    ImageEnabled = collection.ImageEnabled
                }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateItem(CreateItemViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string imgId = "";

                if (viewModel.ItemDetails.ImageFile != null && viewModel.ItemDetails.ImageFile.Length > 0)
                {
                    var content = FormImageContent(viewModel.ItemDetails.ImageFile);

                    var response = await _blobService.ImageUpload(content);
                    if (!response.IsSuccessStatusCode)
                        return RedirectToAction("Error", "Home");

                    imgId = JsonConvert.DeserializeObject<string>(await response.Content.ReadAsStringAsync());
                }

                CollectionItem item = new CollectionItem()
                {
                    Id = Guid.NewGuid(),
                    Name = viewModel.ItemDetails.Name,
                    Description = viewModel.ItemDetails.Description,
                    ImageId = imgId
                };

                if (!_collectionsService.CreateItem(viewModel.ItemDetails.CollectionId, item).Result.IsSuccessStatusCode)
                    return RedirectToAction("Error", "Home");

                return RedirectToAction("Collection", "Collections", new { collectionId = viewModel.ItemDetails.CollectionId });
            }
            else
            {
                return View(viewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditItem(string collectionId, string itemId)
        {
            var response = await _collectionsService.Retrieve(collectionId);
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Error", "Home");

            var collection = JsonConvert.DeserializeObject<Collection>(await response.Content.ReadAsStringAsync());
            var item = collection.CollectionItems.Where(i => i.Id.ToString() == itemId).First();

            return View(new EditItemViewModel()
            {
                CollectionName = collection.Name,
                ItemId = itemId,
                ItemDetails = new ItemEditor()
                {
                    CollectionId = collection.Id,
                    ImageEnabled = collection.ImageEnabled,
                    Name = item.Name,
                    Description = item.Description
                }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItem(EditItemViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var response = await _collectionsService.Retrieve(viewModel.ItemDetails.CollectionId);
                if (!response.IsSuccessStatusCode)
                    return RedirectToAction("Error", "Home");

                var collection = JsonConvert.DeserializeObject<Collection>(await response.Content.ReadAsStringAsync());
                var item = collection.CollectionItems.Where(i => i.Id.ToString() == viewModel.ItemId).First();

                string imgId = item.ImageId;

                if (viewModel.ItemDetails.ImageFile != null && viewModel.ItemDetails.ImageFile.Length > 0)
                {
                    var content = FormImageContent(viewModel.ItemDetails.ImageFile);

                    if(imgId == null || imgId == "")
                    {
                        response = await _blobService.ImageUpload(content);
                        imgId = JsonConvert.DeserializeObject<string>(await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        response = await _blobService.ImageUpdate(imgId, content);
                    }   

                    if (!response.IsSuccessStatusCode)
                        return RedirectToAction("Error", "Home");
                }

                CollectionItem updatedItem = new CollectionItem()
                {
                    Id = new Guid(viewModel.ItemId),
                    Name = viewModel.ItemDetails.Name,
                    Description = viewModel.ItemDetails.Description,
                    ImageId = imgId
                };

                if (!_collectionsService.UpdateItem(viewModel.ItemDetails.CollectionId, updatedItem).Result.IsSuccessStatusCode)
                    return RedirectToAction("Error", "Home");

                return RedirectToAction("Collection", "Collections", new { collectionId = viewModel.ItemDetails.CollectionId });
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
            if (ModelState.IsValid)
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

                        string itemImgId = collection.CollectionItems.Where(i => i.Id.ToString() == item.ItemId).FirstOrDefault().ImageId;

                        if(itemImgId != null && itemImgId != "")
                            imageIds.Add(itemImgId);
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
            else
            {
                return View(viewModel);
            }
        }

        private MultipartFormDataContent FormImageContent(IFormFile imageFile)
        {
            string ImageContentType = imageFile.ContentType;
            var fileName = ContentDispositionHeaderValue.Parse(imageFile.ContentDisposition).FileName;

            if (imageFile.Length > 100000)
            {
                imageFile = _imageManipulation.LoadFile(imageFile).Resize(600).Retrieve();
                ImageContentType = "image/jpg";
            }

            return new MultipartFormDataContent
            {
                {
                    new StreamContent(imageFile.OpenReadStream())
                    {
                        Headers =
                        {
                            ContentLength = imageFile.Length,
                            ContentType = new MediaTypeHeaderValue(ImageContentType)
                        }
                    },
                    "image",
                    fileName
                }
            };
        }
    }
}