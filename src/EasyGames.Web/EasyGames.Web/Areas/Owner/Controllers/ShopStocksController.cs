using System.Linq;
using System.Threading.Tasks;
using EasyGames.Web.Data;
using EasyGames.Web.Services;
using EasyGames.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Web.Areas.Owner.Controllers
{
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

        // Supports:
        //  /Owner/ShopStocks          -> loads first shop
        //  /Owner/ShopStocks?shopId=1 -> uses query
        //  /Owner/ShopStocks/1        -> uses route id
        [HttpGet]
        public async Task<IActionResult> Index(int? shopId, int? id)
        {
            var effectiveId = shopId ?? id;
            if (effectiveId == null || effectiveId.Value <= 0)
            {
                var firstId = await _db.Shops
                    .OrderBy(s => s.Id)
                    .Select(s => s.Id)
                    .FirstOrDefaultAsync();

                if (firstId == 0) return NotFound("No shops available (seed missing).");
                effectiveId = firstId;
            }

            var shop = await _db.Shops.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == effectiveId.Value);
            if (shop == null) return NotFound($"Shop {effectiveId} not found.");

            // Build a simple, strongly-typed label (no reflection)
            string shopLabel =
                !string.IsNullOrWhiteSpace(shop.ShopCode)
                    ? shop.ShopCode
                    : $"{(shop.City ?? "").Trim()}, {(shop.Country ?? "").Trim()}".Trim(' ', ',');

            if (string.IsNullOrWhiteSpace(shopLabel))
                shopLabel = $"Shop #{effectiveId.Value}";

            // Strongly-typed rows (no dynamic)
            var rows = await _db.ShopStocks
                .AsNoTracking()
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

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Inc(int shopId, int productId)
        {
            await _inventory.IncreaseAsync(shopId, productId, 1);
            TempData["msg"] = "Increased by 1.";
            return RedirectToAction(nameof(Index), new { shopId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Dec(int shopId, int productId)
        {
            try
            {
                await _inventory.DecreaseAsync(shopId, productId, 1);
                TempData["msg"] = "Decreased by 1.";
            }
            catch (System.Exception ex)
            {
                TempData["msg"] = "Cannot decrease: " + ex.Message;
            }
            return RedirectToAction(nameof(Index), new { shopId });
        }

        // Diagnostic (optional): /Owner/ShopStocks/Ping?shopId=1
        [HttpGet]
        public IActionResult Ping(int shopId) => Content($"OK ShopStocks (shopId={shopId})");
    }
}


