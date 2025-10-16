using System.Linq;
using System.Threading.Tasks;
using EasyGames.Web.Data;
using EasyGames.Web.Services;
using EasyGames.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Web.Areas.Owner.Controllers
{
    // Area: /Owner/ShopStocks
    [Area("Owner")]
    public class ShopStocksController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IInventoryService _inventory;

        public ShopStocksController(AppDbContext db, IInventoryService inventory)
        {
            _db = db;
            _inventory = inventory;
        }

        
        [HttpGet]
        public async Task<IActionResult> Index(int? shopId, int? id)
        {
            // pick id from ?shopId= or /{id}; else fall back to first seeded shop
            var effectiveId = shopId ?? id;
            if (effectiveId == null || effectiveId.Value <= 0)
            {
                var firstId = await _db.Shops
                    .OrderBy(s => s.Id)
                    .Select(s => s.Id)
                    .FirstOrDefaultAsync(); // returns 0 if none

                if (firstId == 0)
                    return NotFound("No shops available. Seed is missing.");

                effectiveId = firstId;
            }

            // load shop
            var shop = await _db.Shops.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == effectiveId.Value);
            if (shop == null)
                return NotFound($"Shop {effectiveId} not found.");

            // Build a simple label (we avoid assuming Shop has Name)
            var shopLabel = shop.GetType().GetProperty("ShopCode")?.GetValue(shop)?.ToString()
                            ?? $"Shop #{effectiveId.Value}";

            // load stock rows (fallback product label -> "PID <id>")
            var rows = await _db.ShopStocks
                .Where(x => x.ShopId == effectiveId.Value)
                .OrderBy(x => x.ProductId)
                .Select(x => new ShopStockRowVM
                {
                    ProductId = x.ProductId,
                    ProductName = "PID " + x.ProductId, // fallback label
                    Qty = x.Quantity,
                    ReorderLevel = x.ReorderLevel
                })
                .ToListAsync();

            var vm = new ShopStocksVM
            {
                ShopId = effectiveId.Value,
                ShopName = shopLabel,
                Rows = rows,
                Message = TempData["msg"] as string
            };

            return View(vm);
        }

        // POST: /Owner/ShopStocks/Inc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inc(int shopId, int productId)
        {
            // simple +1 (service handles create-or-increment internally if you wrote it that way)
            await _inventory.IncreaseAsync(shopId, productId, 1);
            TempData["msg"] = "Increased by 1.";
            return RedirectToAction(nameof(Index), new { shopId });
        }

        // POST: /Owner/ShopStocks/Dec
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dec(int shopId, int productId)
        {
            try
            {
                await _inventory.DecreaseAsync(shopId, productId, 1); // service enforces no negatives
                TempData["msg"] = "Decreased by 1.";
            }
            catch (System.Exception ex)
            {
                TempData["msg"] = "Cannot decrease: " + ex.Message;
            }
            return RedirectToAction(nameof(Index), new { shopId });
        }

        // Optional quick diagnostic: /Owner/ShopStocks/Ping?shopId=1
        [HttpGet]
        public IActionResult Ping(int shopId) => Content($"OK ShopStocks (shopId={shopId})");
    }
}


