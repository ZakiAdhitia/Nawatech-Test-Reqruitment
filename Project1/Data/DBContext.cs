using Microsoft.EntityFrameworkCore;
using Project1.Models;

namespace Project1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
            modelBuilder.Entity<ProductCategory>().HasQueryFilter(c => c.DeletedAt == null);
            modelBuilder
                .Entity<Product>()
                .HasOne(p => p.ProductCategory)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .IsRequired(false);
            modelBuilder.Entity<Product>().HasQueryFilter(p => p.DeletedAt == null);
        }
    }
}
