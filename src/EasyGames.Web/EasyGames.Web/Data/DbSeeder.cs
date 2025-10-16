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




        }
    }
}



