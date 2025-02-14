using Microsoft.EntityFrameworkCore;
using sportWorld.Models;

namespace sportWorld.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Light-head", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Head-heavy", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Balanced", DisplayOrder = 3 }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product
                { 
                    Id = 1,
                    Name = "Yonex Nanoflare 700 PRO",
                    Description = "The Yonex Nanoflare 700 Pro is designed to make your clears—those deep, high shots that push your opponent to the backcourt—easier and more consistent. Its unique frame design helps you hit these shots with less effort, giving you better control of the game.",
                    Brand = "Yonex",
                    ListPrice = 300,
                    Price = 289,
                    Price20 = 260
                },
                new Product
                { 
                    Id = 2,
                    Name = "Victor Thruster F Ultra",
                    Description = "The New Victor Thruster F Ultra (2024) is made to be user friendly, yet being able to product powerful and pin point accuracy smashes. \r\n\r\nVictor's signature Free Core Handle is used to maximise the racquet's shock absorption. This provides you with a comfortable hitting experience.",
                    Brand = "Victor",
                    ListPrice = 290,
                    Price = 279,
                    Price20 = 250
                },
                new Product
                { 
                    Id = 3,
                    Name = "Yonex Power Cushion Comfort Z 3 (Black/Mint)",
                    Description = "The new update for the Yonex Comfort Z Performance badminton shoes features a couple of upgrades to make the shoes more comfortable, with increased performance.",
                    Brand = "Yonex",
                    ListPrice = 250,
                    Price = 239,
                    Price20 = 210
                }
                );
        }

    }
}
