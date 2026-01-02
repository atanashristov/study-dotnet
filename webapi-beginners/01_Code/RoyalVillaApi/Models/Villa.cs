using System.ComponentModel.DataAnnotations;

namespace RoyalVillaApi.Models
{
    public class Villa
    {
        // By default "Id" will be treated as the primary key
        // [Key]
        public int Id { get; set; }

        // Required attribute makes the property mandatory.
        // By default "required" makes the column non-nullable in the database.
        // [Required]
        public required string Name { get; set; }

        // Default to empty string if not set
        public string Details { get; set; } = default!;
        public double Rate { get; set; }
        public int Sqft { get; set; }
        public int Occupancy { get; set; }

        // Nullable column
        public string? ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}
