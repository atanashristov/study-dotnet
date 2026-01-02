using Microsoft.AspNetCore.Mvc;

namespace RoyalVillaApi.Controllers
{
    [ApiController]
    public class VillaController : ControllerBase
    {
        [HttpGet]
        [Route("villas")]
        public string GetVillas()
        {
            return "List of Villas";
        }
    }
}
