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
    public class CartController : Controller
    {
        private readonly ICartService _cart;
        private readonly AppDbContext _db;

        public CartController(ICartService cart, AppDbContext db)
        {
            _cart = cart;
            _db = db;
        }

        // GET: /Storefront/Cart
        public async Task<IActionResult> Index()
        {
            // 1) Get productId -> qty from session cart
            var map = _cart.Get();

            // 2) Load products for those ids (single round-trip)
            var ids = map.Keys.ToList();
            var products = await _db.Products.AsNoTracking()
                .Where(p => ids.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            // 3) Build rows
            var lines = map.Select(kv =>
            {
                var id = kv.Key;
                var qty = kv.Value;
                products.TryGetValue(id, out var p);

                return new CartRowVM
                {
                    ProductId = id,
                    Name = p?.Name ?? "Unknown",
                    Price = p?.Price ?? 0m,
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

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Inc(int id)
        {
            _cart.Add(id, 1);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Dec(int id)
        {
            _cart.Remove(id, 1);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            _cart.Remove(id, int.MaxValue);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            _cart.Clear();
            return RedirectToAction(nameof(Index));
        }
    }
}


