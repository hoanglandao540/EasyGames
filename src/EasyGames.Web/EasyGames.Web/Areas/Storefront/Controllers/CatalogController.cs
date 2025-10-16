using System.Collections.Generic;
using System.Linq;
using EasyGames.Web.Services;
using EasyGames.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EasyGames.Web.Areas.Storefront.Controllers
{
    [Area("Storefront")]
    public class CatalogController : Controller
    {
        private readonly ICartService _cart;

        // For now we keep a tiny in-memory list (no Product model yet on this branch)
        private static readonly List<CatalogItemVM> SeedProducts = new()
        {
            new CatalogItemVM { Id = 1001, Name = "Galaxy Quest", Price = 49.99m, ImageUrl = "/img/box-a.png" },
            new CatalogItemVM { Id = 1002, Name = "Mystic Adventure", Price = 39.00m, ImageUrl = "/img/box-b.png" },
            new CatalogItemVM { Id = 1003, Name = "Neon Racers", Price = 29.50m, ImageUrl = "/img/box-c.png" }
        };

        public CatalogController(ICartService cart)
        {
            _cart = cart;
        }

        // GET: /Storefront/Catalog
        public IActionResult Index()
        {
            var vm = new CatalogVM
            {
                Items = SeedProducts.ToList(),
                CartCount = _cart.TotalItems()
            };
            return View(vm);
        }

        // POST: /Storefront/Catalog/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(int id)
        {
            _cart.Add(id, 1);                // add one item safely
            TempData["Msg"] = "Added to cart ✅";
            return RedirectToAction(nameof(Index));
        }
    }
}



