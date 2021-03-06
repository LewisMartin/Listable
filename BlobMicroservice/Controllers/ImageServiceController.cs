﻿using System.Threading.Tasks;
using Listable.BlobMicroservice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Listable.BlobMicroservice.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class ImageServiceController : Controller
    {
        private IImageStore _imageStore;

        public ImageServiceController(IImageStore imageStore)
        {
            _imageStore = imageStore;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile image)
        {
            if (image == null)
                return BadRequest();

            try
            {
                using (var stream = image.OpenReadStream())
                {
                    var imageId = await _imageStore.SaveImage(stream);
                    return Json(imageId);
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(string id, IFormFile image)
        {
            if (id == null || id == "" || image == null)
                return BadRequest();

            try
            {
                if (await _imageStore.DeleteImage(id))
                {
                    using (var stream = image.OpenReadStream())
                    {
                        var imageId = await _imageStore.SaveImage(stream, id);

                        if (imageId == id)
                            return StatusCode(StatusCodes.Status200OK);
                    }
                }

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        public IActionResult RetrieveUrl(string id)
        {
            if (id == null || id == "")
                return BadRequest();

            try
            {
                return Json(_imageStore.GetUri(id));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        public IActionResult RetrieveThumbnailUrls(string[] ids)
        {
            if (ids == null)
                return BadRequest();

            try
            {
                return Json(_imageStore.MapThumbnailUris(ids));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || id == "")
                return BadRequest();

            try
            {
                return Json(await _imageStore.DeleteImage(id));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }   
        }
    }
}