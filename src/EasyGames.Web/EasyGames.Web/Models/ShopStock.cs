using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Web.Models
{
    // Stock for a product inside a shop.
    // Unique pair (ShopId, ProductId) ensures "no duplicate stock row" bugs.
    [Index(nameof(ShopId), nameof(ProductId), IsUnique = true)]
    public class ShopStock
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ShopId { get; set; }

        [Required] // Product entity comes later; we still track id now
        public int ProductId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
        public int Quantity { get; set; }

        // Optional simple re-order number (helps future InventoryService)
        [Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; } = 0;

        public Shop? Shop { get; set; } // nav to parent
    }
}


