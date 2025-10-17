using System.Linq;
using System.Threading.Tasks;
using EasyGames.Web.Data;
using EasyGames.Web.Services;
using EasyGames.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Web.Areas.Storefront.Controllers
{
    [Area("Storefront")]
    public class CatalogController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ICartService _cart;

        public CatalogController(AppDbContext db, ICartService cart)
        {
            _db = db;
            _cart = cart;
        }

        // GET: /Storefront/Catalog
        public async Task<IActionResult> Index()
        {
            var items = await _db.Products.AsNoTracking()
                .Where(p => p.IsActive)
                .Select(p => new CatalogItemVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ImageUrl = "/img/placeholder.png"   // <- no dependency on Product.ImageUrl
                })
                .ToListAsync();

            var vm = new CatalogVM
            {
                Items = items,
                CartCount = _cart.TotalItems()
            };

            return View(vm);
        }

        // POST: /Storefront/Catalog/Add
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Add(int id)
        {
            _cart.Add(id, 1);
            TempData["Msg"] = "Added to cart ✅";
            return RedirectToAction(nameof(Index));
        }
    }
}


