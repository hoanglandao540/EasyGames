using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using EasyGames.Web.ViewModels;

namespace EasyGames.Web.Areas.Shop.Controllers
{
    // Area route: /Shop/Pos
    [Area("Shop")]
    public class PosController : Controller
    {
        // We store the POS cart in Session as JSON.
        private const string SessionKey = "pos_cart_v1";

        // GET: /Shop/Pos
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

        // POST: /Shop/Pos/AddLine
        // For now we accept free-form product info (student-level).
        // Later (Step 4) we can hook to real Products/Inventory.
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
                line.Quantity += qty; // simple add
            }

            SaveLines(lines);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Shop/Pos/RemoveLine
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveLine(int productId)
        {
            var lines = LoadLines();
            lines = lines.Where(x => x.ProductId != productId).ToList();
            SaveLines(lines);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Shop/Pos/Clear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            SaveLines(new List<PosLineVM>());
            return RedirectToAction(nameof(Index));
        }

        // POST: /Shop/Pos/Pay
        // SAFE STUB: This just clears the cart and shows a success message.
        // Step 4 will call InventoryService to decrease stock (never negative).
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Pay()
        {
            SaveLines(new List<PosLineVM>());
            TempData["msg"] = "Payment complete (stub). We will update stock in Step 4.";
            return RedirectToAction(nameof(Index));
        }

        // ===== Session helpers =====
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



