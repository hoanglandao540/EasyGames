using System.Linq;
using System.Threading.Tasks;
using EasyGames.Web.Data;
using EasyGames.Web.Models;       // Order / OrderLine
using EasyGames.Web.Services;
using EasyGames.Web.ViewModels;
using Microsoft.AspNetCore.Http;  // Session GetInt32/SetInt32
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Web.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class PosController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ICartService _cart;
        private readonly IInventoryService _inventory;

        // Keep the active ShopId in session so it never falls back to 0
        private const string PosShopIdKey = "POS_ShopId";

        public PosController(AppDbContext db, ICartService cart, IInventoryService inventory)
        {
            _db = db;
            _cart = cart;
            _inventory = inventory;
        }

        // Normalize incoming shopId: if 0/invalid, use session value; default to 1.
        private int ResolveShopId(int shopId)
        {
            if (shopId > 0) return shopId;
            var saved = HttpContext.Session.GetInt32(PosShopIdKey);
            return (saved.HasValue && saved.Value > 0) ? saved.Value : 1;
        }

        // GET: /Shop/Pos
        public async Task<IActionResult> Index(int shopId = 0)
        {
            shopId = ResolveShopId(shopId);
            HttpContext.Session.SetInt32(PosShopIdKey, shopId);

            var map = _cart.Get(); // productId -> qty

            var ids = map.Keys.ToList();
            var products = await _db.Products.AsNoTracking()
                .Where(p => ids.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            var lines = map.Select(kv =>
            {
                var id = kv.Key;
                var qty = kv.Value;
                products.TryGetValue(id, out var p);

                return new PosLineVM
                {
                    ProductId = id,
                    Name = p?.Name ?? "Unknown",
                    Price = p?.Price ?? 0m,
                    Qty = qty
                };
            }).ToList();

            var vm = new PosVM
            {
                ShopId = shopId,
                CustomerPhone = "",
                Lines = lines
            };

            return View(vm);
        }

        // -------------------- POS Cart Endpoints --------------------

        // AddLine (GET)
        [HttpGet, ActionName("AddLine")]
        public IActionResult AddLine_Get(int shopId = 0, int productId = 0, int qty = 1)
        {
            shopId = ResolveShopId(shopId);
            if (productId > 0) _cart.Add(productId, qty <= 0 ? 1 : qty);
            return RedirectToAction(nameof(Index), new { shopId });
        }

        // AddLine (POST)
        [HttpPost, ValidateAntiForgeryToken, ActionName("AddLine")]
        public IActionResult AddLine_Post(int shopId, int productId, int qty)
        {
            shopId = ResolveShopId(shopId);
            if (productId > 0) _cart.Add(productId, qty <= 0 ? 1 : qty);
            return RedirectToAction(nameof(Index), new { shopId });
        }

        // DecLine (GET)
        [HttpGet, ActionName("DecLine")]
        public IActionResult DecLine_Get(int shopId = 0, int productId = 0)
        {
            shopId = ResolveShopId(shopId);
            if (productId > 0) _cart.Remove(productId, 1);
            return RedirectToAction(nameof(Index), new { shopId });
        }

        // DecLine (POST)
        [HttpPost, ValidateAntiForgeryToken, ActionName("DecLine")]
        public IActionResult DecLine_Post(int shopId, int productId)
        {
            shopId = ResolveShopId(shopId);
            if (productId > 0) _cart.Remove(productId, 1);
            return RedirectToAction(nameof(Index), new { shopId });
        }

        // RemoveLine (GET)
        [HttpGet, ActionName("RemoveLine")]
        public IActionResult RemoveLine_Get(int shopId = 0, int productId = 0)
        {
            shopId = ResolveShopId(shopId);
            if (productId > 0) _cart.Remove(productId, int.MaxValue);
            return RedirectToAction(nameof(Index), new { shopId });
        }

        // RemoveLine (POST)
        [HttpPost, ValidateAntiForgeryToken, ActionName("RemoveLine")]
        public IActionResult RemoveLine_Post(int shopId, int productId)
        {
            shopId = ResolveShopId(shopId);
            if (productId > 0) _cart.Remove(productId, int.MaxValue);
            return RedirectToAction(nameof(Index), new { shopId });
        }

        // Clear (GET)
        [HttpGet, ActionName("Clear")]
        public IActionResult Clear_Get(int shopId = 0)
        {
            shopId = ResolveShopId(shopId);
            _cart.Clear();
            return RedirectToAction(nameof(Index), new { shopId });
        }

        // Clear (POST)
        [HttpPost, ValidateAntiForgeryToken, ActionName("Clear")]
        public IActionResult Clear_Post(int shopId)
        {
            shopId = ResolveShopId(shopId);
            _cart.Clear();
            return RedirectToAction(nameof(Index), new { shopId });
        }

        // -------------------- Pay --------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(int shopId, string customerPhone)
        {
            shopId = ResolveShopId(shopId);

            var map = _cart.Get();
            if (map.Count == 0)
            {
                TempData["Msg"] = "Cart is empty.";
                return RedirectToAction(nameof(Index), new { shopId });
            }

            var ids = map.Keys.ToList();
            var products = await _db.Products.AsNoTracking()
                .Where(p => ids.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            var lines = map.Select(kv =>
            {
                var id = kv.Key;
                var qty = kv.Value;
                products.TryGetValue(id, out var p);

                return new PosLineVM
                {
                    ProductId = id,
                    Name = p?.Name ?? "Unknown",
                    Price = p?.Price ?? 0m,
                    Qty = qty
                };
            }).ToList();

            decimal total = lines.Sum(x => x.LineTotal);

            var order = new Order
            {
                Channel = "POS",
                CustomerName = null,
                CustomerEmail = null,
                Phone = customerPhone,
                Total = total,
                Lines = lines.Select(l => new OrderLine
                {
                    ProductId = l.ProductId,
                    Name = l.Name,
                    Price = l.Price,
                    Qty = l.Qty
                }).ToList()
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            bool anyNegative = false;
            foreach (var l in lines)
            {
                await _inventory.DecreaseAsync(shopId, l.ProductId, l.Qty, allowOversell: true);

                var stock = await _db.ShopStocks.AsNoTracking()
                    .FirstOrDefaultAsync(s => s.ShopId == shopId && s.ProductId == l.ProductId);

                if (stock != null && stock.Quantity < 0)
                    anyNegative = true;
            }

            _cart.Clear();

            if (anyNegative)
                TempData["Msg"] = "Order saved, but one or more items went below zero (oversell).";

            return RedirectToAction(nameof(Success));
        }

        public IActionResult Success() => View();
    }
}


