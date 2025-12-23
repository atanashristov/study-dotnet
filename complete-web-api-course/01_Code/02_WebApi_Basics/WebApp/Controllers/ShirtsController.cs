using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Repositories;

namespace WebApp.Controllers
{
    [Route("[controller]")]
    public class ShirtsController : Controller
    {
        private readonly ILogger<ShirtsController> _logger;

        public ShirtsController(ILogger<ShirtsController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(ShirtRepository.GetAllShirts());
        }

        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View("Error!");
        // }
    }
}
