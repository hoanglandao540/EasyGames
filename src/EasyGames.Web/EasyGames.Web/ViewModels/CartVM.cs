using System.Collections.Generic;

namespace EasyGames.Web.ViewModels
{
    // I keep this student-friendly: one line per cart row, plus totals.
    public class CartRowVM
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public decimal LineTotal => Price * Qty;
    }

    public class CartVM
    {
        public List<CartRowVM> Lines { get; set; } = new();
        public int TotalItems { get; set; }
        public decimal GrandTotal { get; set; }
    }
}



