using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(128)]
        public string FirstName { get; set; }

        [MaxLength(128)]
        public string LastName { get; set; }

        [MaxLength(256)]
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }
}
