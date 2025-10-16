using System.Linq;
using EasyGames.Web.Services;
using EasyGames.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EasyGames.Web.Areas.Storefront.Controllers
{
    [Area("Storefront")]
    public class CartController : Controller
    {
        private readonly ICartService _cart;

        public CartController(ICartService cart)
        {
            _cart = cart;
        }

        // GET: /Storefront/Cart
        public IActionResult Index()
        {
            var map = _cart.Get(); // productId -> qty

            var lines = map.Select(kv =>
            {
                var id = kv.Key;
                var qty = kv.Value;

                // Look up product meta from the shared catalog
                ProductCatalog.TryGet(id, out var info);

                return new CartRowVM
                {
                    ProductId = id,
                    Name = info?.Name ?? "Unknown",
                    Price = info?.Price ?? 0m,
                    Qty = qty
                };
            }).ToList();

            var vm = new CartVM
            {
                Lines = lines,
                TotalItems = lines.Sum(x => x.Qty),
                GrandTotal = lines.Sum(x => x.LineTotal)
            };

            return View(vm);
        }

        // POST: /Storefront/Cart/Inc
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Inc(int id)
        {
            _cart.Add(id, 1);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Storefront/Cart/Dec
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Dec(int id)
        {
            _cart.Remove(id, 1); // service clamps to 0, removes when <=0
            return RedirectToAction(nameof(Index));
        }

        // POST: /Storefront/Cart/Remove
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            _cart.Remove(id, int.MaxValue); // remove fully
            return RedirectToAction(nameof(Index));
        }

        // POST: /Storefront/Cart/Clear
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            _cart.Clear();
            return RedirectToAction(nameof(Index));
        }
    }
}



