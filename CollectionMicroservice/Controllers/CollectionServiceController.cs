using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Listable.CollectionMicroservice.DTO;
using Listable.CollectionMicroservice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Listable.CollectionMicroservice.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class CollectionServiceController : Controller
    {
        private ICollectionStore _collectionStore;

        public CollectionServiceController(ICollectionStore collectionStore)
        {
            _collectionStore = collectionStore;
        }

        // GET collections/retrieve
        [HttpGet]
        public JsonResult Retrieve(string collectionId)
        {
            if (collectionId != null)
                return Json(_collectionStore.GetCollection(collectionId));

            return Json("Collection not found");
        }

        // GET collections/retrieveall
        [HttpGet]
        public JsonResult RetrieveAll(string userId)
        {
            if (userId != null)
                return Json(_collectionStore.GetAllCollectionsForUser(userId).ToList());

            return Json(_collectionStore.GetAllCollections().ToList());
        }

        // GET collections/retrieveall
        [HttpGet]
        public JsonResult RetrieveItem(string collectionId, string itemId)
        {
            if (collectionId != null)
                return Json(_collectionStore.GetCollectionItem(collectionId, new Guid(itemId)));

            return Json("Collection not found");
        }

        // POST collections/create
        [HttpPost]
        public async Task<string> Create([FromBody] Collection collection)
        {
            try
            {
                var collections = new List<Collection>();
                collections.Add(collection);

                await _collectionStore.InsertCollections(collections);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "Created test collections";
        }

        // PUT collections/update/5
        [HttpPut]
        public string Update(string id, [FromBody] Collection collection)
        {
            _collectionStore.UpdateCollection(id, collection);

            return "Updated collection: " + id;
        }

        // DELETE collections/delete/5
        [HttpDelete]
        public string Delete(string id)
        {
            var success = _collectionStore.DeleteCollection(id);

            if (success)
                return "Deleted collection: " + id;
            else
                return "Deletion failed";
        }

        [HttpPost]
        public string CreateItem(string collectionId, [FromBody] CollectionItem item)
        {
            var collection = _collectionStore.GetCollection(collectionId);
            collection.CollectionItems.Add(item);

            var success = _collectionStore.UpdateCollection(collectionId, collection);

            if (success)
                return "Added item to collection";
            else
                return "Error";
        }

        [HttpPost]
        public string DeleteItem(string collectionId, [FromBody] List<string> itemIds)
        {
            if (itemIds != null && collectionId != null)
            {
                var collection = _collectionStore.GetCollection(collectionId);

                foreach (var id in itemIds)
                {
                    collection.CollectionItems.RemoveAll(i => i.Id.ToString() == id);
                }

                var success = _collectionStore.UpdateCollection(collectionId, collection);

                if (success)
                    return "Deleted item from collection";
                else
                    return "Error";
            }
            else
            {
                return "Parameters not supplied";
            }
        }
    }
}