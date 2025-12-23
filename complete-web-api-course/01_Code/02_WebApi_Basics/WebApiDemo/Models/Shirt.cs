using System.ComponentModel.DataAnnotations;
using WebApiDemo.Models.Validations;

namespace WebApiDemo.Models
{
    // Entity class - represents domain model
    public class Shirt
    {
        public int ShirtId { get; set; }
        public required string Brand { get; set; }
        public required string Color { get; set; }
        public int? Size { get; set; }
        public required string Gender { get; set; }
        public double? Price { get; set; }
    }

    // For POST
    public class CreateShirtDto : IShirtSizing
    {
        [Required]
        public required string Brand { get; set; }

        [Required]
        public required string Color { get; set; }

        [Shirt_EnsureCorrectSizing]
        public int? Size { get; set; }

        [Required]
        public required string Gender { get; set; }

        public double? Price { get; set; }

        public Shirt ToEntity(int shirtId = 0)
        {
            return new Shirt
            {
                ShirtId = shirtId,
                Brand = Brand,
                Color = Color,
                Size = Size,
                Gender = Gender,
                Price = Price
            };
        }
    }

    // For PUT (full update)
    public class UpdateShirtDto : IShirtSizing
    {
        [Required]
        public required string Brand { get; set; }

        [Required]
        public required string Color { get; set; }

        [Shirt_EnsureCorrectSizing]
        public int? Size { get; set; }

        [Required]
        public required string Gender { get; set; }

        public double? Price { get; set; }

        public void ApplyToEntity(Shirt shirt)
        {
            shirt.Brand = Brand;
            shirt.Color = Color;
            shirt.Size = Size;
            shirt.Gender = Gender;
            shirt.Price = Price;
        }
    }

    // For PATCH (partial update - no required attributes)
    public class PartialUpdateShirtDto : IShirtSizing
    {
        public string? Brand { get; set; }
        public string? Color { get; set; }

        [Shirt_EnsureCorrectSizing]
        public int? Size { get; set; }

        public string? Gender { get; set; }
        public double? Price { get; set; }

        public void ApplyToEntity(Shirt shirt)
        {
            if (!string.IsNullOrWhiteSpace(Brand))
            {
                shirt.Brand = Brand;
            }
            if (!string.IsNullOrWhiteSpace(Color))
            {
                shirt.Color = Color;
            }
            if (Size.HasValue)
            {
                shirt.Size = Size;
            }
            if (!string.IsNullOrWhiteSpace(Gender))
            {
                shirt.Gender = Gender;
            }
            if (Price.HasValue)
            {
                shirt.Price = Price;
            }
        }
    }
}
