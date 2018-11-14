using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            var collections = JsonConvert.DeserializeObject<List<Collection>>(response.Content.ReadAsStringAsync().Result);

            if (collections == null)
                return NotFound();

            return Ok(collections);
        }  
        
        [HttpPost]
        public IActionResult CreateCollection([FromBody] CreateCollectionModel model)
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

        private string GetUserSub()
        {
            string sub = "";

            foreach (var identity in User.Identities)
            {
                sub = identity.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault().Value;
            }

            return sub;
        }

    }
}
