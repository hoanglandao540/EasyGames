namespace EasyGames.Web.Models
{
    public class OrderLine
    {
        public int Id { get; set; }
        public int OrderId { get; set; }      // FK by convention
        public int ProductId { get; set; }

        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int Qty { get; set; }

        public Order? Order { get; set; }     // nav back to Order
    }
}



