using System.Collections.Generic;
using System.Linq;

namespace EasyGames.Web.Services
{
    public class ProductInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }

    public static class ProductCatalog
    {
        private static readonly List<ProductInfo> _items = new()
        {
            new ProductInfo { Id = 1001, Name = "Galaxy Quest",      Price = 49.99m, ImageUrl = "/img/box-a.png" },
            new ProductInfo { Id = 1002, Name = "Mystic Adventure",   Price = 39.00m, ImageUrl = "/img/box-b.png" },
            new ProductInfo { Id = 1003, Name = "Neon Racers",        Price = 29.50m, ImageUrl = "/img/box-c.png" }
        };

        public static IReadOnlyList<ProductInfo> GetAll() => _items;

        public static bool TryGet(int id, out ProductInfo? item)
        {
            item = _items.FirstOrDefault(p => p.Id == id);
            return item != null;
        }
    }
}



