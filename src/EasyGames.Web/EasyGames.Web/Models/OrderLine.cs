namespace EasyGames.Web.Models
{
    public class OrderLine
    {
        public int Id { get; set; }

        public int OrderId { get; set; }      // FK (EF will link by convention)
        public int ProductId { get; set; }

        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int Qty { get; set; }

        public decimal LineTotal => Price * Qty;
    }
}


