using Microsoft.AspNetCore.Mvc;
using WebApiDemo.Data;
using WebApiDemo.Filters;
using WebApiDemo.Models;

namespace WebApiDemo.Controllers
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
            var shirt = HttpContext.Items["shirt"] as Shirt;

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
        [TypeFilter(typeof(Shirt_ValidateUpdateShirtFilterAttribute))]
        [TypeFilter(typeof(Shirt_HandleUpdateExceptionsFilterAttribute))]
        public IActionResult UpdateShirt(int id, [FromBody] UpdateShirtDto updateShirtDto)
        {
            var shirtToUpdate = HttpContext.Items["shirt"] as Shirt;

            updateShirtDto.ApplyToEntity(shirtToUpdate!);
            db.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        [TypeFilter(typeof(Shirt_ValidatePatchShirtFilterAttribute))]
        [TypeFilter(typeof(Shirt_HandleUpdateExceptionsFilterAttribute))]
        public IActionResult PartialUpdateShirt(int id, [FromBody] PartialUpdateShirtDto partialUpdateShirtDto)
        {
            var shirtToUpdate = HttpContext.Items["shirt"] as Shirt;

            partialUpdateShirtDto.ApplyToEntity(shirtToUpdate!);
            db.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        public IActionResult DeleteShirt(int id)
        {
            var shirtToDelete = HttpContext.Items["shirt"] as Shirt;

            db.Shirts.Remove(shirtToDelete!);
            db.SaveChanges();

            return Ok(shirtToDelete);
        }
    }
}
