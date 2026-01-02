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

        [HttpGet("{id:int}")]
        public string GetVillaById(int id)
        {
            return "Villa Details of Id: " + id;
        }

        [HttpGet("{id:int}/{name}")]
        public string GetVillaByIdAndName([FromRoute] int id, [FromRoute] string name)
        {
            return "Villa Details of Id: " + id + " and Name: " + name;
        }

    }
}
