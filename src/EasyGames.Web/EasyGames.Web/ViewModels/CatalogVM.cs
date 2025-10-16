using System.Collections.Generic;

namespace EasyGames.Web.ViewModels
{
    public class CatalogItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }    // placeholder for now
    }

    public class CatalogVM
    {
        public List<CatalogItemVM> Items { get; set; } = new();
        public int CartCount { get; set; }       // show live cart badge
    }
}



