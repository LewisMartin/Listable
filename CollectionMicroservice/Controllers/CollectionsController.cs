using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionMicroservice.DTO;
using CollectionMicroservice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CollectionMicroservice.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class CollectionsController : Controller
    {
        private CollectionStore _collectionStore;

        public CollectionsController(CollectionStore collectionStore)
        {
            _collectionStore = collectionStore;
        }

        // GET collections/retrieve
        [HttpGet]
        public JsonResult Retrieve()
        {
            return Json(_collectionStore.GetAllCollections().ToList()); 
        }

        [HttpGet]
        public JsonResult RetrieveSpecific()
        {
            return Json(_collectionStore.GetAllCollectionsForUser("TestUser").ToList());
        }

        // POST collections/create
        [HttpPost]
        public async Task<string> Create([FromBody] string data)
        {
            var collections = JsonConvert.DeserializeObject<List<Collection>>(data, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy HH:mm:ss" });

            await _collectionStore.InsertCollections(collections);

            return "Created test collections";
        }

        // PUT collections/update/5
        [HttpPut]
        public string Update(int id, [FromBody]string value)
        {
            return "Updated collection: " + value;
        }

        // DELETE collections/delete/5
        [HttpDelete]
        public string Delete(int id)
        {
            return "Deleted collection: " + id;
        }
    }
}