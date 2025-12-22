using Microsoft.AspNetCore.Mvc;
using WebAppiDemo.Data;
using WebAppiDemo.Filters;
using WebAppiDemo.Models;
using WebAppiDemo.Models.Repositories;

namespace WebAppiDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShirtsController : ControllerBase
    {
        private readonly ApplicationDbContext db;

        public ShirtsController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetShirts()
        {
            return Ok(db.Shirts.ToList());
        }

        [HttpGet("{id}")]
        // Now that the filter accepts parameter in constructor we cannot use this
        // [Shirt_ValidateShirtIdFilter]
        // We do TypeFilter instead
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        public IActionResult GetShirtById(int id)
        {
            // var shirt = db.Shirts.Find(id);
            // We don't want to query the database again, so we get the shirt from HttpContext.Items
            var shirt = HttpContext.Items["Shirt"] as Shirt;

            return Ok(shirt);
        }

        [HttpPost]
        [TypeFilter(typeof(Shirt_ValidateCreateShirtFilterAttribute))]
        public IActionResult CreateShirt([FromBody] CreateShirtDto createShirtDto)
        {
            var newShirt = createShirtDto.ToEntity();
            db.Shirts.Add(newShirt);
            db.SaveChanges();

            // In a real application, you would save the shirt to a database
            return CreatedAtAction(nameof(GetShirtById),
                new { id = newShirt.ShirtId },
                newShirt);
            // return Created($"/shirts/{newShirt.ShirtId}", newShirt);
        }

        [HttpPut("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        [Shirt_ValidateUpdateShirtFilter]
        [Shirt_HandleUpdateExceptionsFilter]
        public IActionResult UpdateShirt(int id, [FromBody] UpdateShirtDto updateShirtDto)
        {
            // var existingShirt = ShirtRepository.GetShirtById(id);
            var existingShirt = HttpContext.Items["Shirt"] as Shirt;

            updateShirtDto.ApplyToEntity(existingShirt!);

            ShirtRepository.UpdateShirt(existingShirt!);

            return NoContent();
        }

        [HttpPatch("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        [Shirt_ValidatePatchShirtFilter]
        [Shirt_HandleUpdateExceptionsFilter]
        public IActionResult PartialUpdateShirt(int id, [FromBody] PartialUpdateShirtDto partialUpdateShirtDto)
        {
            // var existingShirt = ShirtRepository.GetShirtById(id);
            var existingShirt = HttpContext.Items["Shirt"] as Shirt;

            partialUpdateShirtDto.ApplyToEntity(existingShirt!);

            ShirtRepository.UpdateShirt(existingShirt!);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        public IActionResult DeleteShirt(int id)
        {
            // var existingShirt = ShirtRepository.GetShirtById(id);
            var existingShirt = HttpContext.Items["Shirt"] as Shirt;

            ShirtRepository.DeleteShirt(id);

            return Ok(existingShirt);
        }
    }
}
