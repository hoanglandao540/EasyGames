using System.ComponentModel.DataAnnotations;

namespace EasyGames.Web.ViewModels
{
    // Keep it student-simple: only fields we KNOW exist on Shop
    public class ShopCreateVM
    {
        [Required, StringLength(20)]
        public string ShopCode { get; set; } = "";

        [Required, StringLength(64)]
        public string City { get; set; } = "";

        [Required, StringLength(64)]
        public string Country { get; set; } = "";

        [StringLength(32)]
        public string? Phone { get; set; }
    }
}


