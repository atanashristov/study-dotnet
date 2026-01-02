using Microsoft.EntityFrameworkCore;
using RoyalVillaApi.Models;

namespace RoyalVillaApi.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Villa> Villas { get; set; }
    }
}
