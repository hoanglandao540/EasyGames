using System.Linq;
using System.Threading.Tasks;
using EasyGames.Web.Data;
using EasyGames.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Web.Areas.Owner.Controllers
{
    // async, guard rails, friendly messages.
    [Area("Owner")]
    public class ShopStocksController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IInventoryService _inv;

        public ShopStocksController(AppDbContext db, IInventoryService inv)
        {
            _db = db;
            _inv = inv;
        }

        // GET: /Owner/ShopStocks?shopId=1
        public async Task<IActionResult> Index(int shopId)
        {
            var shop = await _db.Shops
                .Include(s => s.Stocks)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == shopId);

            if (shop == null) return NotFound();

            ViewBag.ShopId = shop.Id;
            ViewBag.ShopName = $"{shop.ShopCode} — {shop.ShopName}";

            // no Product entity yet, so we show ProductId as placeholder
            var rows = shop.Stocks
                .OrderBy(x => x.ProductId)
                .ToList();

            return View(rows);
        }

        // POST: /Owner/ShopStocks/Increase
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Increase(int shopId, int productId, int byQty = 1)
        {
            var ok = await _inv.IncreaseAsync(shopId, productId, byQty);
            TempData["Msg"] = ok ? $"Increased P#{productId} by {byQty}." : "Increase failed.";
            return RedirectToAction(nameof(Index), new { shopId });
        }

        // POST: /Owner/ShopStocks/Decrease
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decrease(int shopId, int productId, int byQty = 1)
        {
            var ok = await _inv.DecreaseAsync(shopId, productId, byQty);
            TempData["Msg"] = ok ? $"Decreased P#{productId} by {byQty}." : "Not enough stock.";
            return RedirectToAction(nameof(Index), new { shopId });
        }
    }
}



