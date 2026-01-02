using Microsoft.EntityFrameworkCore;

namespace RoyalVillaApi.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {

        // Define your DbSets here
        // public DbSet<YourEntity> YourEntities { get; set; }
    }
}
