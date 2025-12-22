using Microsoft.EntityFrameworkCore;
using WebAppiDemo.Models;

namespace WebAppiDemo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Shirt> Shirts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Here we can map the name of "ShirtId" to "Id" in the database
            // builder.Entity<Shirt>(entity =>
            // {
            //     entity.ToTable("Shirts");
            //     entity.HasKey(e => e.ShirtId);
            //     entity.Property(e => e.ShirtId).HasColumnName("Id");
            // });

            // data seeding

            modelBuilder.Entity<Shirt>().HasData(
                new Shirt { ShirtId = 1, Brand = "Nike", Color = "Red", Size = 10, Gender = "Male", Price = 29.99 },
                new Shirt { ShirtId = 2, Brand = "Adidas", Color = "Blue", Size = 12, Gender = "Male", Price = 34.99 },
                new Shirt { ShirtId = 3, Brand = "Puma", Color = "Green", Size = 8, Gender = "Female", Price = 24.99 }
            );
        }
    }
}
