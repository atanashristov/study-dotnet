using Microsoft.AspNetCore.Mvc;

namespace RoyalVillaApi.Controllers
{
    [Route("api/villa")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        [HttpGet]
        public string GetVillas()
        {
            return "List of Villas";
        }
    }
}
