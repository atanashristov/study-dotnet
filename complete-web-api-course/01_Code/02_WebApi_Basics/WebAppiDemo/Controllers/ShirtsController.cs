using Microsoft.AspNetCore.Mvc;
using WebAppiDemo.Models;
using WebAppiDemo.Models.Validations;

namespace WebAppiDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShirtsController : ControllerBase
    {
        private static List<Shirt> shirts = new List<Shirt>
            {
                new Shirt { ShirtId = 1, Brand = "Nike", Color = "Red", Size = 10, Gender = "Male", Price = 29.99 },
                new Shirt { ShirtId = 2, Brand = "Adidas", Color = "Blue", Size = 12, Gender = "Male", Price = 34.99 },
                new Shirt { ShirtId = 3, Brand = "Puma", Color = "Green", Size = 8, Gender = "Female", Price = 24.99 }
            };

        [HttpGet]
        public IActionResult GetShirts()
        {
            return Ok(shirts);
        }

        [HttpGet("{id}")]
        public IActionResult GetShirtById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid shirt ID.");
            }

            var shirt = shirts.FirstOrDefault(s => s.ShirtId == id);
            if (shirt == null)
            {
                return NotFound();
            }
            return Ok(shirt);
        }

        [HttpPost]
        public IActionResult CreateShirt([FromBody] CreateShirtDto createShirtDto)
        {
            var newShirtId = shirts.Max(s => s.ShirtId) + 1;
            var newShirt = createShirtDto.ToEntity(newShirtId);

            shirts.Add(newShirt);

            // In a real application, you would save the shirt to a database
            return Created($"/shirts/{newShirt.ShirtId}", newShirt);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateShirt(int id, [FromBody] UpdateShirtDto updateShirtDto)
        {
            var existingShirt = shirts.FirstOrDefault(s => s.ShirtId == id);
            if (existingShirt == null)
            {
                return NotFound();
            }

            updateShirtDto.ApplyToEntity(existingShirt);

            // In a real application, you would update the shirt in a database
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartialUpdateShirt(int id, [FromBody] PartialUpdateShirtDto partialUpdateShirtDto)
        {
            var existingShirt = shirts.FirstOrDefault(s => s.ShirtId == id);
            if (existingShirt == null)
            {
                return NotFound();
            }

            // Validate size/gender combination (check both directions)
            if (partialUpdateShirtDto.Size.HasValue || partialUpdateShirtDto.Gender != null)
            {
                var gender = partialUpdateShirtDto.Gender ?? existingShirt.Gender;
                var size = partialUpdateShirtDto.Size ?? existingShirt.Size;

                if (!ShirtSizeValidator.IsValidSize(gender, size, out var errorMessage))
                {
                    ModelState.AddModelError("Size", errorMessage ?? "Invalid size for the selected gender.");
                    return ValidationProblem(ModelState);
                }
            }

            partialUpdateShirtDto.ApplyToEntity(existingShirt);

            // In a real application, you would partially update the shirt in a database
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteShirt(int id)
        {
            var existingShirt = shirts.FirstOrDefault(s => s.ShirtId == id);
            if (existingShirt == null)
            {
                return NotFound();
            }

            shirts.Remove(existingShirt);

            // In a real application, you would delete the shirt from a database
            return NoContent();
        }
    }
}
