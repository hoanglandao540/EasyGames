using EasyGames.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Shop> Shops => Set<Shop>();
        public DbSet<ShopStock> ShopStocks => Set<ShopStock>();

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderLine> OrderLines => Set<OrderLine>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderLine>()
                .HasOne(l => l.Order)
                .WithMany(o => o.Lines)
                .HasForeignKey(l => l.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ShopStock>()
                .HasIndex(s => new { s.ShopId, s.ProductId })
                .IsUnique();
        }
    }
}


