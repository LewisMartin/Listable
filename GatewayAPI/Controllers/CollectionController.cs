using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GatewayAPI.Models.Collection;
using GatewayAPI.Models.Collection.Forms;
using GatewayAPI.Services;
using Listable.CollectionMicroservice.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GatewayAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class CollectionController : Controller
    {
        private readonly IImageManipulation _imageManipulation;
        private readonly IBlobService _blobService;
        private readonly ICollectionsService _collectionsService;

        public CollectionController(IBlobService blobService, ICollectionsService collectionsService)
        {
            _blobService = blobService;
            _collectionsService = collectionsService;
        }

        [HttpGet]
        public IActionResult GetCollectionsForAuthenticatedUser()
        {
            return GetCollections(GetUserSub());
        }

        [HttpGet]
        public IActionResult GetCollections(string userId)
        {
            if (userId == null)
                return BadRequest();

            var response = _collectionsService.RetrieveAll(userId).Result;

            if (!response.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var collections = JsonConvert.DeserializeObject<List<CollectionsListItem>>(response.Content.ReadAsStringAsync().Result);

            if (collections == null)
                return NotFound();

            return Ok(collections);
        }  
        
        [HttpGet]
        public async Task<IActionResult> GetCollection(string id)
        {
            if (id == null)
                return BadRequest();

            var response = _collectionsService.Retrieve(id).Result;

            if (!response.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var collection = JsonConvert.DeserializeObject<Collection>(response.Content.ReadAsStringAsync().Result);

            var thumbnailMap = new Dictionary<string, string>();
            if(collection.DisplayFormat == CollectionDisplayFormat.Grid)
            {
                response = MapThumbnails(collection);

                if (!response.IsSuccessStatusCode)
                    return StatusCode(StatusCodes.Status500InternalServerError);

                thumbnailMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(await response.Content.ReadAsStringAsync());
            }

            return Ok(new CollectionView()
            {
                Id = collection.Id,
                Name = collection.Name,
                GridDisplay = collection.DisplayFormat == CollectionDisplayFormat.Grid ? true : false,
                CollectionViewItems = collection.CollectionItems.Select(item => new CollectionViewItem()
                {
                    Id = item.Id.ToString(),
                    Name = item.Name,
                    ThumbnailUri = thumbnailMap.ContainsKey(item.ImageId) ? thumbnailMap[item.ImageId] : ""
                }).ToList()
            });
        }
        
        [HttpPost]
        public IActionResult CreateCollection([FromBody] CreateCollectionFormModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Collection collection = new Collection()
            {
                Name = model.Name,
                Owner = GetUserSub(),
                ImageEnabled = model.ImageEnabled,
                DisplayFormat = model.ImageEnabled ? (model.GridDisplay == true ? CollectionDisplayFormat.Grid : CollectionDisplayFormat.List) : CollectionDisplayFormat.List,
                CollectionItems = new List<CollectionItem>()
            };

            var response = _collectionsService.Create(collection).Result;

            if (!response.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            collection = JsonConvert.DeserializeObject<Collection>(response.Content.ReadAsStringAsync().Result);

            return Ok(collection);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCollection([FromBody] DeleteCollectionFormModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _collectionsService.Retrieve(model.SelectedCollectionId);
            if (!response.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var collection = JsonConvert.DeserializeObject<Collection>(await response.Content.ReadAsStringAsync());

            if (collection.Owner != GetUserSub())
                return Unauthorized();

            if (collection.ImageEnabled)
            {
                foreach (var item in collection.CollectionItems)
                {
                    if (item.ImageId != null && item.ImageId != "")
                    {
                        if (!_blobService.ImageDelete(item.ImageId).Result.IsSuccessStatusCode)
                            return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }
            }

            if (!_collectionsService.Delete(model.SelectedCollectionId).Result.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetCollectionItem(string collectionId, string itemId)
        {
            if (collectionId == null || itemId == null)
                return BadRequest();

            var response = await _collectionsService.Retrieve(collectionId);
            if (!response.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var collection = JsonConvert.DeserializeObject<Collection>(await response.Content.ReadAsStringAsync());

            var item = collection.CollectionItems.Where(i => i.Id == new Guid(itemId)).FirstOrDefault();

            if (item == null)
                return RedirectToAction("Error", "Home");

            var url = "";
            if (collection.ImageEnabled && item.ImageId != null && item.ImageId != "")
            {
                response = await _blobService.ImageRetrieveUrl(item.ImageId);
                if (!response.IsSuccessStatusCode)
                    return StatusCode(StatusCodes.Status500InternalServerError);

                url = JsonConvert.DeserializeObject<string>(await response.Content.ReadAsStringAsync());
            }

            return Ok(new CollectionItemView()
            {
                CollectionId = collection.Id,
                CollectionName = collection.Name,
                Id = item.Id.ToString(),
                Name = item.Name,
                Description = item.Description,
                ShowImage = collection.ImageEnabled,
                ImageUrl = url
            });
        }


        private string GetUserSub()
        {
            string sub = "";

            foreach (var identity in User.Identities)
            {
                sub = identity.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault().Value;
            }

            return sub;
        }

        private HttpResponseMessage MapThumbnails(Collection collection)
        {
            List<string> imgIds = new List<string>();
            foreach (var item in collection.CollectionItems)
            {
                if (item.ImageId != null && item.ImageId != "")
                    imgIds.Add(item.ImageId);
            }

            return _blobService.ImageRetrieveThumbs(imgIds).Result;
        }
    }
}
