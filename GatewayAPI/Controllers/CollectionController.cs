using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
