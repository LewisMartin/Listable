using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollectionMicroservice.DTO;
using CollectionMicroservice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<string> Create()
        {
            var data = new List<Collection>
            {
                new Collection()
                {
                    Id = "3",
                    Name = "3rd collection",
                    Owner = "randomuser",
                    CollectionItems = new List<CollectionItem>()
                    {
                        new CollectionItem() { Name = "Item 1" },
                    }
                },
                new Collection()
                {
                    Id = "4",
                    Name = "final collection",
                    Owner = "nobody",
                    CollectionItems = new List<CollectionItem>()
                    {
                        new CollectionItem() { Name = "Item 1" },
                        new CollectionItem() { Name = "Item 2" },
                        new CollectionItem() { Name = "Item 3" },
                        new CollectionItem() { Name = "Item 4" }
                    }
                }
            };

            await _collectionStore.InsertCollections(data);

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