using Microsoft.AspNetCore.Mvc;
using WebAppiDemo.Filters;
using WebAppiDemo.Models;
using WebAppiDemo.Models.Repositories;

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
        [Shirt_ValidateShirtIdFilter]
        public IActionResult GetShirtById(int id)
        {
            return Ok(ShirtRepository.GetShirtById(id));
        }

        [HttpPost]
        [Shirt_ValidateCreateShirtFilter]
        public IActionResult CreateShirt([FromBody] CreateShirtDto createShirtDto)
        {
            var newShirt = ShirtRepository.AddShirt(createShirtDto.ToEntity());

            // In a real application, you would save the shirt to a database
            return CreatedAtAction(nameof(GetShirtById),
                new { id = newShirt.ShirtId },
                newShirt);
            // return Created($"/shirts/{newShirt.ShirtId}", newShirt);
        }

        [HttpPut("{id}")]
        [Shirt_ValidateShirtIdFilter]
        [Shirt_ValidateUpdateShirtFilter]
        [Shirt_HandleUpdateExceptionsFilter]
        public IActionResult UpdateShirt(int id, [FromBody] UpdateShirtDto updateShirtDto)
        {
            var existingShirt = ShirtRepository.GetShirtById(id);

            updateShirtDto.ApplyToEntity(existingShirt!);

            ShirtRepository.UpdateShirt(existingShirt!);

            return NoContent();
        }

        [HttpPatch("{id}")]
        [Shirt_ValidateShirtIdFilter]
        [Shirt_ValidatePatchShirtFilter]
        [Shirt_HandleUpdateExceptionsFilter]
        public IActionResult PartialUpdateShirt(int id, [FromBody] PartialUpdateShirtDto partialUpdateShirtDto)
        {
            var existingShirt = ShirtRepository.GetShirtById(id);

            partialUpdateShirtDto.ApplyToEntity(existingShirt!);

            ShirtRepository.UpdateShirt(existingShirt!);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Shirt_ValidateShirtIdFilter]
        public IActionResult DeleteShirt(int id)
        {
            var existingShirt = ShirtRepository.GetShirtById(id);

            ShirtRepository.DeleteShirt(id);

            return Ok(existingShirt);
        }
    }
}
