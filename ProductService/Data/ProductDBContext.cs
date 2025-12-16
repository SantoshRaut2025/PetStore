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

            modelBuilder.Entity<PetStoreUser>(entity =>
            {
                entity.HasKey(e => e.ID).HasName("PetStoreUsers_pkey");

                entity.Property(e => e.ID).ValueGeneratedNever();
            });
        }        
        public DbSet<Product> Products { get; set; } = null!;


        public virtual DbSet<PetStoreUser> PetStoreUsers { get; set; }
    }
}
