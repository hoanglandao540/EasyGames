using System;
using System.Threading;
using System.Threading.Tasks;
using EasyGames.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Web.Data
{
    // InMemory provider for now (super fast to start). I also auto-set CreatedAtUtc / UpdatedAtUtc so audit data stays correct.
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Shop> Shops => Set<Shop>();
        public DbSet<ShopStock> ShopStocks => Set<ShopStock>();
        public DbSet<AppUser> Users { get; set; } = null!;
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderLine> OrderLines => Set<OrderLine>();




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Shop 1- ShopStocks (many)
            modelBuilder.Entity<Shop>()
                .HasMany(s => s.Stocks)
                .WithOne(ss => ss.Shop!)
                .HasForeignKey(ss => ss.ShopId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Sku)
                .IsUnique();


            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

        // UpdatedAtUtc auto-bump on every change; CreatedAtUtc only on insert
        public override int SaveChanges()
        {
            TouchAuditStamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            TouchAuditStamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void TouchAuditStamps()
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<Shop>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAtUtc = now;
                    entry.Entity.UpdatedAtUtc = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAtUtc = now;
                }
            }
        }
    }
}



