using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Web.Models
{
    // One physical shop. Names simple but add real-world checks
  
    [Index(nameof(ShopCode), IsUnique = true)]  // each shop has a unique short code
    public class Shop
    {
        [Key]
        public int Id { get; set; } // PK (int keeps student life easy)

        [Required, StringLength(16)]
        public string ShopCode { get; set; } = ""; // like "DHK-01" (unique, short and sweet)

        [Required, StringLength(120)]
        public string ShopName { get; set; } = ""; // store name

        [StringLength(160)]
        public string? AddressLine1 { get; set; }

        [StringLength(64)]
        public string? City { get; set; }

        [StringLength(64)]
        public string? Country { get; set; }

        [Phone, StringLength(24)]
        public string? Phone { get; set; }

        // Simple flags + audit fields (teachers usually like this)
        public bool IsActive { get; set; } = true;
        public bool IsWarehouse { get; set; }   // true for the owner’s inventory

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow; // set in code too
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow; // auto-bumped on save

        [Timestamp]                           
        public byte[]? RowVersion { get; set; }

        // Navigation: one shop has many stock rows (each product + qty)
        public List<ShopStock> Stocks { get; set; } = new();
    }
}


