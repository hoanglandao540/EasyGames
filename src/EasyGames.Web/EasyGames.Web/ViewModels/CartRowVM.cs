namespace EasyGames.Web.ViewModels
{
    public class CartRowVM
    {
        // Primary id
        public int Id { get; set; }

        // Alias for places that expect ProductId
        public int ProductId
        {
            get => Id;
            set => Id = value;
        }

        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int Qty { get; set; }

        // Line total (computed) but allow assignment if some code tries to set it
        private decimal? _lineTotalOverride;
        public decimal LineTotal
        {
            get => _lineTotalOverride ?? (Price * Qty);
            set => _lineTotalOverride = value;
        }
    }
}


