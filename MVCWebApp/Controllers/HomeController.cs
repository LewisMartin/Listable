using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Listable.MVCWebApp.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Microsoft.Extensions.Caching.Distributed;
using Listable.MVCWebApp.Services;

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
