using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models
{
    public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        [MaxLength(256)]
        public string Description { get; set; }

        [MaxLength(64)]
        public string Group { get; set; }
    }
}
