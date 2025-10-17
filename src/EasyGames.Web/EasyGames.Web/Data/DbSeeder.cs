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
            // Ensure warehouse exists
            if (!db.Shops.Any(s => s.IsWarehouse))
            {
                db.Shops.Add(new Shop { ShopCode = "WH-OWNER", City = "Darwin", Country = "Australia", Phone = "+61 8 7000 0000", IsWarehouse = true });
                db.SaveChanges();
            }

            if (!db.Products.Any())
            {
                db.Products.AddRange(
                    new Product { Sku = "EG-001", Name = "Galaxy Quest", ImageUrl = "/img/1001.png", CostPrice = 25m, SellPrice = 49.99m, IsActive = true },
                    new Product { Sku = "EG-002", Name = "Mystic Adventure", ImageUrl = "/img/1002.png", CostPrice = 20m, SellPrice = 39.00m, IsActive = true },
                    new Product { Sku = "EG-003", Name = "Neon Racers", ImageUrl = "/img/1003.png", CostPrice = 12m, SellPrice = 29.50m, IsActive = true }
                );
                db.SaveChanges();
            }

            // (optional) seed warehouse stock into ShopStocks for the warehouse shop
            var wh = db.Shops.First(s => s.IsWarehouse);
            if (!db.ShopStocks.Any(ss => ss.ShopId == wh.Id))
            {
                var p = db.Products.ToList();
                db.ShopStocks.AddRange(
                    new ShopStock { ShopId = wh.Id, ProductId = p[0].Id, Quantity = 50, ReorderLevel = 5 },
                    new ShopStock { ShopId = wh.Id, ProductId = p[1].Id, Quantity = 50, ReorderLevel = 5 },
                    new ShopStock { ShopId = wh.Id, ProductId = p[2].Id, Quantity = 50, ReorderLevel = 5 }
                );
                db.SaveChanges();
            }




<<<<<<< HEAD


            // We'll reference simple fake product IDs for now (1001..1003).
            var stocks = new[]
{
    new ShopStock { Id = 1, ShopId = 1, ProductId = 1, Quantity = 15, ReorderLevel = 5 },
    new ShopStock { Id = 2, ShopId = 1, ProductId = 2, Quantity = 8,  ReorderLevel = 4 },
    new ShopStock { Id = 3, ShopId = 1, ProductId = 3, Quantity = 0,  ReorderLevel = 6 },
};

            db.Shops.Add(darwin);
            db.ShopStocks.AddRange(stocks);
            db.SaveChanges();

            // Tiny console message helps during F5 runs
            Console.WriteLine("[Seed] Darwin shop created with 3 stock rows.");
=======
>>>>>>> origin/feature/akshata/db-skeleton
        }
    }
}



