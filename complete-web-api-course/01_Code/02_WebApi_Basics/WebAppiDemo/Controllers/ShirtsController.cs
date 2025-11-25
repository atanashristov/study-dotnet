using Microsoft.AspNetCore.Mvc;
using WebAppiDemo.Models;
using WebAppiDemo.Models.Repositories;
using WebAppiDemo.Models.Validations;

namespace WebAppiDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShirtsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetShirts()
        {
            return Ok(ShirtRepository.GetAllShirts());
        }

        [HttpGet("{id}")]
        public IActionResult GetShirtById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid shirt ID.");
            }

            var shirt = ShirtRepository.GetShirtById(id);
            if (shirt == null)
            {
                return NotFound();
            }
            return Ok(shirt);
        }

        [HttpPost]
        public IActionResult CreateShirt([FromBody] CreateShirtDto createShirtDto)
        {
            var newShirt = ShirtRepository.AddShirt(createShirtDto.ToEntity());

            // In a real application, you would save the shirt to a database
            return Created($"/shirts/{newShirt.ShirtId}", newShirt);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateShirt(int id, [FromBody] UpdateShirtDto updateShirtDto)
        {
            var existingShirt = ShirtRepository.GetShirtById(id);
            if (existingShirt == null)
            {
                return NotFound();
            }

            updateShirtDto.ApplyToEntity(existingShirt);

            // In a real application, you would update the shirt in a database
            ShirtRepository.SaveShirt(existingShirt);

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartialUpdateShirt(int id, [FromBody] PartialUpdateShirtDto partialUpdateShirtDto)
        {
            var existingShirt = ShirtRepository.GetShirtById(id);
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

            // In a real application, you would update the shirt in a database
            ShirtRepository.SaveShirt(existingShirt);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteShirt(int id)
        {
            var existingShirt = ShirtRepository.GetShirtById(id);
            if (existingShirt == null)
            {
                return NotFound();
            }

            // In a real application, you would delete the shirt from a database
            ShirtRepository.DeleteShirt(id);

            return NoContent();
        }
    }
}
