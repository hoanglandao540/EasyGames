using System;
using System.Collections.Generic;

namespace EasyGames.Web.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
        public string Channel { get; set; } = "Online";  // "Online" | "POS"
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? Phone { get; set; }
        public decimal Total { get; set; }

        public List<OrderLine> Lines { get; set; } = new();
    }
}


