using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EasyGames.Web.ViewModels;
using EasyGames.Web.Data;
using EasyGames.Web.Services;

namespace EasyGames.Web.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class PosController : Controller
    {
        private const string SessionKey = "pos_cart_v1";
        private readonly AppDbContext _db;
        private readonly IInventoryService _inventory;

        // We inject Db + Inventory so we can decrease stock on Pay
        public PosController(AppDbContext db, IInventoryService inventory)
        {
            _db = db;
            _inventory = inventory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var vm = new PosVM
            {
                Lines = LoadLines(),
                Message = TempData["msg"] as string
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddLine(int productId, string productName, decimal price, int qty)
        {
            if (qty < 1) qty = 1;

            var lines = LoadLines();
            var line = lines.FirstOrDefault(x => x.ProductId == productId);
            if (line == null)
            {
                lines.Add(new PosLineVM
                {
                    ProductId = productId,
                    ProductName = productName ?? "",
                    UnitPrice = price,
                    Quantity = qty
                });
            }
            else
            {
                line.Quantity += qty;
            }

            SaveLines(lines);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveLine(int productId)
        {
            var lines = LoadLines().Where(x => x.ProductId != productId).ToList();
            SaveLines(lines);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            SaveLines(new List<PosLineVM>());
            return RedirectToAction(nameof(Index));
        }

        // NOW does real stock decrease (never negative – service enforces)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay()
        {
            var lines = LoadLines();
            if (lines.Count == 0)
            {
                TempData["msg"] = "No items to pay.";
                return RedirectToAction(nameof(Index));
            }

            var shop = _db.Shops.FirstOrDefault();
            if (shop == null)
            {
                TempData["msg"] = "Shop not found (seed missing).";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Decrease each product stock
                foreach (var ln in lines)
                {
                    // InventoryService from Akshata handles 'no negatives'
                    await _inventory.DecreaseAsync(shop.Id, ln.ProductId, ln.Quantity);
                }

                SaveLines(new List<PosLineVM>());
                TempData["msg"] = "Payment complete. Stock updated.";
            }
            catch (System.Exception ex)
            {
                TempData["msg"] = "Payment failed: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        //  Session helpers 
        private List<PosLineVM> LoadLines()
        {
            var str = HttpContext.Session.GetString(SessionKey);
            if (string.IsNullOrWhiteSpace(str)) return new List<PosLineVM>();

            var lines = JsonSerializer.Deserialize<List<PosLineVM>>(str,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return lines ?? new List<PosLineVM>();
        }

        private void SaveLines(List<PosLineVM> lines)
        {
            var json = JsonSerializer.Serialize(lines);
            HttpContext.Session.SetString(SessionKey, json);
        }
    }
}



