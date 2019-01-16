using System;
using System.Collections.Generic;
using System.Linq;
using Listable.CollectionMicroservice.DTO;
using Listable.CollectionMicroservice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Retrieve(string collectionId)
        {
            if(collectionId == null || collectionId == "")
                return BadRequest();

            var collection = _collectionStore.GetCollection(collectionId);

            if (collection == null)
                return NotFound();
            else
                return Json(collection);
        }

        // GET collections/retrieveall
        [HttpGet]
        public IActionResult RetrieveAll(int userId)
        {
            return Json(_collectionStore.GetAllCollectionsForUser(userId).ToList());
        }

        // GET collections/query
        [HttpPost]
        public IActionResult Query([FromBody] CollectionQuery query)
        {
            if (query == null)
                return BadRequest();

            return Json(_collectionStore.QueryCollections(query).ToList());
        }

        // GET collections/retrieveall
        [HttpGet]
        public IActionResult RetrieveItem(string collectionId, string itemId)
        {
            if (collectionId == null || collectionId == "" || itemId == null || itemId == "")
                return BadRequest();

            var item = _collectionStore.GetCollectionItem(collectionId, new Guid(itemId));

            if (item == null)
                return NotFound();

            return Json(item);
        }

        // POST collections/create
        [HttpPost]
        public IActionResult Create([FromBody] Collection collection)
        {
            if (collection == null)
                return BadRequest();

            try
            {
                collection = _collectionStore.InsertCollection(collection);

                if(collection == null)
                    return StatusCode(StatusCodes.Status500InternalServerError);

                return Json(collection);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // PUT collections/update/5
        [HttpPut]
        public IActionResult Update(string id, [FromBody] Collection collection)
        {
            if (id == null || id == "" || collection == null)
                return BadRequest();

            if (_collectionStore.UpdateCollection(id, collection))
                return Ok();
            else
                return StatusCode(StatusCodes.Status500InternalServerError);
        }

        // DELETE collections/delete/5
        [HttpDelete]
        public IActionResult Delete(string id)
        {
            if (id == null || id == "")
                return BadRequest();

            if (_collectionStore.DeleteCollection(id))
                return Ok();
            else
                return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPost]
        public IActionResult CreateItem(string collectionId, [FromBody] CollectionItem item)
        {
            if (collectionId == null || collectionId == "" || item == null)
                return BadRequest();

            var collection = _collectionStore.GetCollection(collectionId);
            collection.CollectionItems.Add(item);

            if (_collectionStore.UpdateCollection(collectionId, collection))
                return Ok();
            else
                return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPut]
        public IActionResult UpdateItem(string collectionId, [FromBody] CollectionItem item)
        {
            if (collectionId == null || collectionId == "" || item == null)
                return BadRequest();

            var collection = _collectionStore.GetCollection(collectionId);
            var existingItem = collection.CollectionItems.Where(i => i.Id == item.Id).First();

            existingItem.Name = item.Name;
            existingItem.Description = item.Description;
            existingItem.ImageId = item.ImageId;

            if (_collectionStore.UpdateCollection(collectionId, collection))
                return Ok();
            else
                return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPost]
        public IActionResult DeleteItem(string collectionId, [FromBody] List<string> itemIds)
        {
            if (collectionId == null || collectionId == "" || itemIds == null)
                return BadRequest();

            var collection = _collectionStore.GetCollection(collectionId);

            foreach (var id in itemIds)
            {
                collection.CollectionItems.RemoveAll(i => i.Id.ToString() == id);
            }

            if (_collectionStore.UpdateCollection(collectionId, collection))
                return Ok();
            else
                return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}