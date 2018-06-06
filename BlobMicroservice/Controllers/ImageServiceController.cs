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

        [HttpGet]
        public String Index()
        {
            return "Index";
        }

        // POST collections/create
        [HttpPost]
        public async Task<string> Upload(IFormFile image)
        {
            return "Image recieved";
        }
    }
}