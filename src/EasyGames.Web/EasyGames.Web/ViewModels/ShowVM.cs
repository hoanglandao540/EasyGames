using System;

namespace EasyGames.Web.ViewModels
{
    public class ShopVM
    {
        public int Id { get; set; }
        public string ShopCode { get; set; } = "";
        public string ShopName { get; set; } = "";
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }

        // Calculated summary fields below
        public int TotalSkus { get; set; }     // how many different product rows
        public int TotalQty { get; set; }      // sum of all quantities
        public int LowStockSkus { get; set; }  // rows with qty <= reorder level
    }
}



