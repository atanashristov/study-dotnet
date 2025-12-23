using Microsoft.AspNetCore.Mvc;
using WebApp.Data;
using WebApp.Models;
using WebApp.Models.Repositories;

namespace WebApp.Controllers
{
    [Route("[controller]")]
    public class ShirtsController : Controller
    {
        private readonly ILogger<ShirtsController> _logger;
        private readonly IWebApiExecuter _webApiExecuter;

        public ShirtsController(
            ILogger<ShirtsController> logger,
            IWebApiExecuter webApiExecuter)
        {
            _logger = logger;
            _webApiExecuter = webApiExecuter;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _webApiExecuter.GetAsync<List<Shirt>>("shirts"));
        }

        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View("Error!");
        // }
    }
}
