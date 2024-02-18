using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(128)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        public decimal Salary { get; set; }
    }
}
