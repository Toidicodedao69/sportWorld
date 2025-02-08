using Microsoft.EntityFrameworkCore;
using sportWorld.Models;

namespace sportWorld.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Light-head", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Head-heavy", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Balanced", DisplayOrder = 3 }
                );
        }
    }
}
