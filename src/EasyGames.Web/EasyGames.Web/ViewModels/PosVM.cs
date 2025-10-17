using System.Collections.Generic;
using System.Linq;

namespace EasyGames.Web.ViewModels
{
    public class PosLineVM
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public decimal LineTotal => Price * Qty;

        // aliases for any old views
        public string ProductName { get => Name; set => Name = value; }
        public decimal UnitPrice { get => Price; set => Price = value; }
        public int Quantity { get => Qty; set => Qty = value; }
    }

    public class PosVM
    {
        public int ShopId { get; set; } = 1;
        public string CustomerPhone { get; set; } = "";
        public List<PosLineVM> Lines { get; set; } = new();
        public int TotalItems => Lines?.Sum(x => x.Qty) ?? 0;
        public decimal GrandTotal => Lines?.Sum(x => x.LineTotal) ?? 0m;
        public string? Message { get; set; }
    }
}


