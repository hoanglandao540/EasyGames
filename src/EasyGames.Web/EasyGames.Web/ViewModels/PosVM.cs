using System.Collections.Generic;
using System.Linq;

namespace EasyGames.Web.ViewModels
{
    // One POS line
    public class PosLineVM
    {
        // Our canonical names
        public int ProductId { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int Qty { get; set; }

        public decimal LineTotal => Price * Qty;

        // ---- Aliases to match existing views ----
        // If your Razor uses ProductName / UnitPrice / Quantity,
        // these aliases will keep it compiling without changing the views.
        public string ProductName { get => Name; set => Name = value; }
        public decimal UnitPrice { get => Price; set => Price = value; }
        public int Quantity { get => Qty; set => Qty = value; }
    }

    // Whole POS screen model
    public class PosVM
    {
        public int ShopId { get; set; } = 1;
        public string CustomerPhone { get; set; } = "";

        public List<PosLineVM> Lines { get; set; } = new();

        // Computed totals
        public int TotalItems => Lines?.Sum(x => x.Qty) ?? 0;
        public decimal GrandTotal => Lines?.Sum(x => x.LineTotal) ?? 0m;

        // ---- Alias for views expecting a Message field ----
        public string? Message { get; set; }
    }
}


