using Domain;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public class ApplicationDbContext 
        : IdentityDbContext<
            ApplicationUser, // Inherited from IdentityUser, so we can customize the tables
            ApplicationRole, // Inherited from IdentityRole, so we can customize the tables
            string, // type of the primary key is string, even if it is guid
            IdentityUserClaim<string>, // claims = permissions. type of the primary key is string, even if it is guid
            IdentityUserRole<string>, // type of the primary key is string, even if it is guid
            IdentityUserLogin<string>, // type of the primary key is string even if it is guid
            ApplicationRoleClaim, 
            IdentityUserToken<string> // type of the primary key is string, even if it is guid
         >
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

        public DbSet<Employee> Employees => Set<Employee>();
    }
}
