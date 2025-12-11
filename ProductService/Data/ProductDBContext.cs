using Microsoft.EntityFrameworkCore;
using ProductService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ProductService.Data
{   
    public class ProductDBContext : IdentityDbContext<User>
    {
        public ProductDBContext(DbContextOptions<ProductDBContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().ToTable("Products");
        }
        public DbSet<Product> Products { get; set; } = null!;
    }
}
