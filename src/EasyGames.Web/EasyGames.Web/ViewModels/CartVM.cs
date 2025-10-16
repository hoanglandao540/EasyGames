using System.Collections.Generic;
using System.Linq;

namespace EasyGames.Web.ViewModels
{
    public class CartVM
    {
        // Storage used by services/controllers
        public List<CartRowVM> Lines { get; set; } = new();

        // Alias so views can use Items or Lines interchangeably
        public List<CartRowVM> Items
        {
            get => Lines;
            set => Lines = value ?? new List<CartRowVM>();
        }

        // Totals (computed) but allow assignment if any controller sets them
        private decimal? _grandTotalOverride;
        public decimal GrandTotal
        {
            get => _grandTotalOverride ?? Lines.Sum(l => l.Price * l.Qty);
            set => _grandTotalOverride = value;
        }

        private int? _totalItemsOverride;
        public int TotalItems
        {
            get => _totalItemsOverride ?? Lines.Sum(l => l.Qty);
            set => _totalItemsOverride = value;
        }
    }
}


