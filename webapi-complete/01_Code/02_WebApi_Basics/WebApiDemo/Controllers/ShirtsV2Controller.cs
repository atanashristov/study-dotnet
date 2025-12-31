using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using WebApiDemo.Attributes;
using WebApiDemo.Data;
using WebApiDemo.Filters;
using WebApiDemo.Filters.AuthFilters;
using WebApiDemo.Models;

namespace WebApiDemo.Controllers
{
  [ApiController]
  [Route("api/Shirts")]
  [ApiVersion("2.0")]
  [JwtTokenAuthFilter]
  public class ShirtsV2Controller : ControllerBase
  {
    private readonly ApplicationDbContext db;

    public ShirtsV2Controller(ApplicationDbContext db)
    {
      this.db = db;
    }

    [HttpGet]
    [RequiredClaim("read", "true")]
    public IActionResult GetShirts()
    {
      // V2 enhancement: Return shirts with additional metadata
      var shirts = db.Shirts.ToList();
      var response = new
      {
        Version = "2.0",
        Count = shirts.Count,
        Data = shirts,
        Meta = new
        {
          TotalValue = shirts.Sum(s => s.Price),
          AveragePrice = shirts.Any() ? shirts.Average(s => s.Price) : 0,
          Brands = shirts.Select(s => s.Brand).Distinct().ToList()
        }
      };
      return Ok(response);
    }

    [HttpGet("{id}")]
    [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
    [RequiredClaim("read", "true")]
    public IActionResult GetShirtById(int id)
    {
      var shirt = HttpContext.Items["shirt"] as Shirt;

      // V2 enhancement: Return shirt with related information
      var response = new
      {
        Version = "2.0",
        Data = shirt,
        Meta = new
        {
          RequestedAt = DateTime.UtcNow,
          Currency = "USD"
        }
      };
      return Ok(response);
    }

    [HttpPost]
    [TypeFilter(typeof(Shirt_ValidateCreateShirtFilterAttribute))]
    [RequiredClaim("write", "true")]
    public IActionResult CreateShirt([FromBody] CreateShirtDto createShirtDto)
    {
      var newShirt = createShirtDto.ToEntity();
      // V2 enhancement: Set creation timestamp
      newShirt.CreatedAt = DateTime.UtcNow;

      db.Shirts.Add(newShirt);
      db.SaveChanges();

      var response = new
      {
        Version = "2.0",
        Data = newShirt,
        Meta = new
        {
          CreatedAt = newShirt.CreatedAt,
          Message = "Shirt created successfully in API v2.0"
        }
      };

      return CreatedAtAction(nameof(GetShirtById),
          new { id = newShirt.ShirtId, version = "2.0" },
          response);
    }

    [HttpPut("{id}")]
    [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
    [TypeFilter(typeof(Shirt_ValidateUpdateShirtFilterAttribute))]
    [TypeFilter(typeof(Shirt_HandleUpdateExceptionsFilterAttribute))]
    [RequiredClaim("write", "true")]
    public IActionResult UpdateShirt(int id, [FromBody] UpdateShirtDto updateShirtDto)
    {
      var shirtToUpdate = HttpContext.Items["shirt"] as Shirt;

      updateShirtDto.ApplyToEntity(shirtToUpdate!);
      // V2 enhancement: Track last modified time
      shirtToUpdate!.LastModified = DateTime.UtcNow;

      db.SaveChanges();

      // V2 returns success message instead of NoContent
      var response = new
      {
        Version = "2.0",
        Message = "Shirt updated successfully",
        Meta = new
        {
          UpdatedAt = shirtToUpdate.LastModified,
          ShirtId = shirtToUpdate.ShirtId
        }
      };

      return Ok(response);
    }

    [HttpPatch("{id}")]
    [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
    [TypeFilter(typeof(Shirt_ValidatePatchShirtFilterAttribute))]
    [TypeFilter(typeof(Shirt_HandleUpdateExceptionsFilterAttribute))]
    [RequiredClaim("write", "true")]
    public IActionResult PartialUpdateShirt(int id, [FromBody] PartialUpdateShirtDto partialUpdateShirtDto)
    {
      var shirtToUpdate = HttpContext.Items["shirt"] as Shirt;

      partialUpdateShirtDto.ApplyToEntity(shirtToUpdate!);
      // V2 enhancement: Track last modified time
      shirtToUpdate!.LastModified = DateTime.UtcNow;

      db.SaveChanges();

      // V2 returns success message instead of NoContent
      var response = new
      {
        Version = "2.0",
        Message = "Shirt partially updated successfully",
        Meta = new
        {
          UpdatedAt = shirtToUpdate.LastModified,
          ShirtId = shirtToUpdate.ShirtId,
          PatchedFields = partialUpdateShirtDto.GetType().GetProperties()
                  .Where(p => p.GetValue(partialUpdateShirtDto) != null)
                  .Select(p => p.Name)
                  .ToList()
        }
      };

      return Ok(response);
    }

    [HttpDelete("{id}")]
    [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
    [RequiredClaim("delete", "true")]
    public IActionResult DeleteShirt(int id)
    {
      var shirtToDelete = HttpContext.Items["shirt"] as Shirt;

      db.Shirts.Remove(shirtToDelete!);
      db.SaveChanges();

      // V2 enhancement: Return deletion confirmation with metadata
      var response = new
      {
        Version = "2.0",
        Message = "Shirt deleted successfully",
        Data = shirtToDelete,
        Meta = new
        {
          DeletedAt = DateTime.UtcNow,
          DeletedById = "System" // Could be enhanced to use actual user ID
        }
      };

      return Ok(response);
    }

    // V2 exclusive endpoint - Get shirts statistics
    [HttpGet("statistics")]
    [RequiredClaim("read", "true")]
    public IActionResult GetShirtsStatistics()
    {
      var shirts = db.Shirts.ToList();

      var statistics = new
      {
        Version = "2.0",
        GeneratedAt = DateTime.UtcNow,
        TotalShirts = shirts.Count,
        TotalValue = shirts.Sum(s => s.Price),
        AveragePrice = shirts.Any() ? shirts.Average(s => s.Price) : 0,
        MaxPrice = shirts.Any() ? shirts.Max(s => s.Price) : 0,
        MinPrice = shirts.Any() ? shirts.Min(s => s.Price) : 0,
        BrandStatistics = shirts.GroupBy(s => s.Brand)
              .Select(g => new
              {
                Brand = g.Key,
                Count = g.Count(),
                AveragePrice = g.Average(s => s.Price),
                TotalValue = g.Sum(s => s.Price)
              }).ToList(),
        SizeDistribution = shirts.GroupBy(s => s.Size)
              .Select(g => new
              {
                Size = g.Key,
                Count = g.Count()
              }).OrderBy(x => x.Size).ToList(),
        GenderDistribution = shirts.GroupBy(s => s.Gender)
              .Select(g => new
              {
                Gender = g.Key,
                Count = g.Count()
              }).ToList()
      };

      return Ok(statistics);
    }
  }
}
