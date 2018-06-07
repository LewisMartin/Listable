using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlobMicroservice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlobMicroservice.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public class ImageServiceController : Controller
    {
        private ImageStore _imageStore;

        public ImageServiceController(ImageStore imageStore)
        {
            _imageStore = imageStore;
        }

        [HttpPost]
        public async Task<string> Upload(IFormFile image)
        {
            if (image != null)
            {
                using (var stream = image.OpenReadStream())
                {
                    var imageId = await _imageStore.SaveImage(stream);
                    return imageId;
                }
            }
            else
            {
                return "Image was null";
            }
        }

        [HttpGet]
        public String RetrieveUrl(string id)
        {
            return _imageStore.GetUri(id);
        }

        [HttpDelete]
        public async Task<bool> Delete(string id)
        {
            return await _imageStore.DeleteImage(id);
        }
    }
}