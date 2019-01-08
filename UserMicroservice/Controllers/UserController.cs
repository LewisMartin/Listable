using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Listable.UserMicroservice.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : Controller
    {
        [HttpGet]
        public IActionResult GetUser()
        {
            return Json("Username");
        }
    }
}
