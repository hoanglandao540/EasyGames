using System.Collections.Generic;

namespace EasyGames.Web.ViewModels
{
    // One row in the stock table
    public class ShopStockRowVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = ""; // fallback label until you join Products
        public int Qty { get; set; }
        public int ReorderLevel { get; set; }
    }

    // Page view model
    public class ShopStocksVM
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; } = "";
        public List<ShopStockRowVM> Rows { get; set; } = new List<ShopStockRowVM>();
        public string? Message { get; set; }
    }
}


