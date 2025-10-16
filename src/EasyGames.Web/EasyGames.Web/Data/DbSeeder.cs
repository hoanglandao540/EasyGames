using System;
using System.Linq;
using EasyGames.Web.Models;

namespace EasyGames.Web.Data
{
    // I created Very small data seeder so the app has something real to show.
    public static class DbSeeder
    {
        public static void Seed(AppDbContext db)
        {
            // If we already have data, do nothing (safe to call multiple times).
            if (db.Shops.Any()) return;

            var darwin = new Shop
            {
                Id = 1,      // fixed IDs are okay for seed
                ShopCode = "DRW-01",
                ShopName = "EasyGames Darwin",
                AddressLine1 = "24 Smith St",
                City = "Darwin",
                Country = "Australia",
                Phone = "+61 8 7000 1234",
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            // We'll reference simple fake product IDs for now (1001..1003).
            var stocks = new[]
            {
                new ShopStock { Id = 1, ShopId = 1, ProductId = 1001, Quantity = 15, ReorderLevel = 5 },
                new ShopStock { Id = 2, ShopId = 1, ProductId = 1002, Quantity = 8,  ReorderLevel = 4 },
                new ShopStock { Id = 3, ShopId = 1, ProductId = 1003, Quantity = 0,  ReorderLevel = 6 }, 
            };

            db.Shops.Add(darwin);
            db.ShopStocks.AddRange(stocks);
            db.SaveChanges();

            // Tiny console message helps during F5 runs
            Console.WriteLine("[Seed] Darwin shop created with 3 stock rows.");
        }
    }
}



