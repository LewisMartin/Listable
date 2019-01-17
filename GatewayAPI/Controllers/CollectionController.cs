using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GatewayAPI.Models.Collection;
using GatewayAPI.Models.Collection.Forms;
using GatewayAPI.Services;
using Listable.CollectionMicroservice.DTO;
using Listable.UserMicroservice.DTO;
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
        private readonly IUserService _userService;

        public CollectionController(IImageManipulation imageManipulation, IBlobService blobService, ICollectionsService collectionsService, IUserService userService)
        {
            _imageManipulation = imageManipulation;
            _blobService = blobService;
            _collectionsService = collectionsService;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetCollectionsForAuthenticatedUser()
        {
            return GetCollections(GetUserId());
        }

        [HttpGet]
        public IActionResult GetCollections(int userId)
        {
            var response = _collectionsService.RetrieveAll(userId).Result;

            if (!response.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var collections = JsonConvert.DeserializeObject<List<CollectionsListItem>>(response.Content.ReadAsStringAsync().Result);

            if (collections == null)
                return NotFound();

            return Ok(collections);
        }  
        
        [HttpPost]
        public IActionResult QueryCollections([FromBody] CollectionQueryFormModel model)
        {
            var response = _collectionsService.Query(new CollectionQuery() {
                SearchTerm = model.SearchTerm
            }).Result;

            if (!response.IsSuccessStatusCode)
                return BadRequest();

            var collections = JsonConvert.DeserializeObject<List<Collection>>(response.Content.ReadAsStringAsync().Result);

            if (collections == null)
                return NotFound();

            var queryResults = new List<CollectionQueryResult>();

            foreach (var collection in collections)
            {
                queryResults.Add(new CollectionQueryResult()
                {
                    Id = collection.Id,
                    Name = collection.Name,
                    CollectionSize = collection.CollectionItems.Count(),
                    ImageEnabled = collection.ImageEnabled
                });
            }

            return Json(new CollectionQueryResults()
            {
                Count = queryResults.Count(),
                QueryResults = queryResults
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetCollection(string id)
        {
            if (id == null)
                return BadRequest();

            if (!UserHasPermissionsOnCollection(id, PermissionType.View))
                return Forbid();

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
                    ThumbnailUri = (item.ImageId != null) && (thumbnailMap.ContainsKey(item.ImageId)) ? thumbnailMap[item.ImageId] : ""
                }).ToList()
            });
        }

        [HttpGet]
        public IActionResult GetCollectionSettings(string id)
        {
            if (id == null)
                return BadRequest();

            if (!UserHasPermissionsOnCollection(id, PermissionType.Edit))
                return Forbid();

            var response = _collectionsService.Retrieve(id).Result;

            if (!response.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var collection = JsonConvert.DeserializeObject<Collection>(response.Content.ReadAsStringAsync().Result);

            return Ok(new CollectionSettings()
            {
                Id = collection.Id,
                Name = collection.Name,
                PrivateMode = collection.PrivateMode,
                ImageEnabled = collection.ImageEnabled,
                GridDisplay = collection.DisplayFormat == CollectionDisplayFormat.Grid ? true : false
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
                Owner = GetUserId(),
                PrivateMode = model.PrivateMode,
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
        public async Task<IActionResult> EditCollection([FromBody] EditCollectionFormModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!UserHasPermissionsOnCollection(model.Id, PermissionType.Edit))
                return Forbid();

            var response = await _collectionsService.Retrieve(model.Id);
            if (!response.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var collection = JsonConvert.DeserializeObject<Collection>(await response.Content.ReadAsStringAsync());

            collection.Name = model.Name;
            collection.PrivateMode = model.PrivateMode;
            collection.DisplayFormat = model.GridDisplay == true ? CollectionDisplayFormat.Grid : CollectionDisplayFormat.List;

            if (!_collectionsService.Update(collection).Result.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCollection([FromBody] DeleteCollectionFormModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!UserHasPermissionsOnCollection(model.SelectedCollectionId, PermissionType.Edit))
                return Forbid();

            var response = await _collectionsService.Retrieve(model.SelectedCollectionId);
            if (!response.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var collection = JsonConvert.DeserializeObject<Collection>(await response.Content.ReadAsStringAsync());

            if (collection.Owner != GetUserId())
                return Forbid();

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

            if (!UserHasPermissionsOnCollection(collectionId, PermissionType.View))
                return Forbid();

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

        [HttpPost]
        public async Task<IActionResult> CreateCollectionItem([FromForm] CreateCollectionItemFormModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!UserHasPermissionsOnCollection(model.CollectionId, PermissionType.Edit))
                return Forbid();

            string imgId = "";
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var content = FormImageContent(model.ImageFile);

                var response = await _blobService.ImageUpload(content);
                if (!response.IsSuccessStatusCode)
                    return StatusCode(StatusCodes.Status500InternalServerError);

                imgId = JsonConvert.DeserializeObject<string>(await response.Content.ReadAsStringAsync());
            }

            CollectionItem item = new CollectionItem()
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                ImageId = imgId
            };

            if (!_collectionsService.CreateItem(model.CollectionId, item).Result.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> EditCollectionItem([FromForm] EditCollectionItemFormModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!UserHasPermissionsOnCollection(model.CollectionId, PermissionType.Edit))
                return Forbid();

            var response = await _collectionsService.RetrieveItem(model.CollectionId, model.Id);
            if (!response.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var item = JsonConvert.DeserializeObject<CollectionItem>(await response.Content.ReadAsStringAsync());

            string imgId = item.ImageId;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var content = FormImageContent(model.ImageFile);

                if (imgId == null || imgId == "")
                {
                    response = await _blobService.ImageUpload(content);
                    imgId = JsonConvert.DeserializeObject<string>(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    response = await _blobService.ImageUpdate(imgId, content);
                }

                if (!response.IsSuccessStatusCode)
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }

            CollectionItem updatedItem = new CollectionItem()
            {
                Id = new Guid(model.Id),
                Name = model.Name,
                Description = model.Description,
                ImageId = imgId
            };

            if (!_collectionsService.UpdateItem(model.CollectionId, updatedItem).Result.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCollectionItem([FromBody] DeleteCollectionItemFormModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!UserHasPermissionsOnCollection(model.CollectionId, PermissionType.Edit))
                return Forbid();

            var response = await _collectionsService.Retrieve(model.CollectionId);
            if (!response.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            var collection = JsonConvert.DeserializeObject<Collection>(await response.Content.ReadAsStringAsync());

            var itemIds = new List<string>();
            itemIds.Add(model.CollectionItemId);

            string itemImageId = collection.CollectionItems.Where(i => i.Id.ToString() == model.CollectionItemId).FirstOrDefault().ImageId;

            if (collection.ImageEnabled && itemImageId != null && itemImageId != "")
            {
                if (!_blobService.ImageDelete(itemImageId).Result.IsSuccessStatusCode)
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var content = JsonConvert.SerializeObject(itemIds);

            if (!_collectionsService.DeleteItem(model.CollectionId, content).Result.IsSuccessStatusCode)
                return StatusCode(StatusCodes.Status500InternalServerError);

            return Ok();
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

        private int GetUserId()
        {
            var response = _userService.GetUserBySub(GetUserSub()).Result;

            var user = JsonConvert.DeserializeObject<UserDetails>(response.Content.ReadAsStringAsync().Result);

            return user.Id;
        }

        private bool UserHasPermissionsOnCollection(string collectionId, PermissionType permType)
        {
            var response = _collectionsService.CheckPermissions(GetUserId(), collectionId, permType).Result;

            if (!response.IsSuccessStatusCode)
                return false;

            return true;
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
