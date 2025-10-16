using System.ComponentModel.DataAnnotations;

namespace EasyGames.Web.ViewModels
{
    public class ProductVM
    {
        public int Id { get; set; }

        [Required, StringLength(30)]
        public string Sku { get; set; } = "";

        [Required, StringLength(120)]
        public string Name { get; set; } = "";

        [Range(0, 100000)]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;
    }
}


