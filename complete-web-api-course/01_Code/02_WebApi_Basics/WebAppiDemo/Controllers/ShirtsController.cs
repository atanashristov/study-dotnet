using Microsoft.AspNetCore.Mvc;
using WebAppiDemo.Models;

namespace WebAppiDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShirtsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetShirts()
        {
            var shirts = new[]
            {
                new { Id = 1, Color = "Red", Size = "M" },
                new { Id = 2, Color = "Blue", Size = "L" },
                new { Id = 3, Color = "Green", Size = "S" }
            };
            return Ok(shirts);
        }

        [HttpGet("{id}")]
        public IActionResult GetShirtById(int id, [FromHeader(Name = "color")] string? color)
        {
            var shirt = new { Id = id, Color = color ?? "Random", Size = "M" };
            return Ok(shirt);
        }

        [HttpPost]
        public IActionResult CreateShirt([FromBody] Shirt shirt)
        {
            shirt.ShirtId = 4;
            // In a real application, you would save the shirt to a database
            return Created("/shirts/4", shirt);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateShirt(int id)
        {
            // In a real application, you would update the shirt in a database
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartialUpdateShirt(int id)
        {
            // In a real application, you would partially update the shirt in a database
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteShirt(int id)
        {
            // In a real application, you would delete the shirt from a database
            return NoContent();
        }
    }
}
