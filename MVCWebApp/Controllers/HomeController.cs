using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace Listable.MVCWebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private static readonly HttpClient Client = new HttpClient();
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration, IDistributedCache cache)
        {
            _configuration = configuration;
            _cache = cache;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Collections"); ;
        }
    }
}
