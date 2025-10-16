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

        public CatalogController(ICartService cart)
        {
            _cart = cart;
        }

        // GET: /Storefront/Catalog
        public IActionResult Index()
        {
            // Maps the shared ProductCatalog into the VM used by the view
            var vm = new CatalogVM
            {
                Items = ProductCatalog.GetAll()
                        .Select(p => new CatalogItemVM
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Price = p.Price,
                            ImageUrl = p.ImageUrl
                        })
                        .ToList(),
                CartCount = _cart.TotalItems()
            };
            return View(vm);
        }

        // POST: /Storefront/Catalog/Add
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add(int id)
        {
            _cart.Add(id, 1); // add one item safely
            TempData["Msg"] = "Added to cart ✅";
            return RedirectToAction(nameof(Index));
        }
    }
}



