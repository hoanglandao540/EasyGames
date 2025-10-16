using System.Collections.Generic;
using System.Linq;

namespace EasyGames.Web.ViewModels
{
    // A single line in the POS cart (student-level simple model)
    public class PosLineVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }

    // The full POS page view model
    public class PosVM
    {
        public List<PosLineVM> Lines { get; set; } = new List<PosLineVM>();
        public string? Message { get; set; }
        public decimal GrandTotal => Lines.Sum(x => x.LineTotal);
    }
}



