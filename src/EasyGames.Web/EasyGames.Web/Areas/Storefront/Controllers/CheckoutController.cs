using System.Linq;
using System.Threading.Tasks;
using EasyGames.Web.Data;
using EasyGames.Web.Models;
using EasyGames.Web.Services;
using EasyGames.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Web.Areas.Storefront.Controllers
{
    [Area("Storefront")]
    public class CheckoutController : Controller
    {
        private readonly ICartService _cart;
        private readonly AppDbContext _db;

        public CheckoutController(ICartService cart, AppDbContext db)
        {
            _cart = cart;
            _db = db;
        }

        // GET: /Storefront/Checkout
        public async Task<IActionResult> Index()
        {
            var vm = await BuildVmFromCartAsync();
            if (!vm.Lines.Any())
            {
                TempData["Msg"] = "Your cart is empty. Please add items first.";
                return RedirectToAction("Index", "Catalog");
            }
            return View(vm);
        }

        // POST: /Storefront/Checkout
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CheckoutVM form)
        {
            var vm = await BuildVmFromCartAsync();
            form.Lines = vm.Lines;
            form.TotalItems = vm.TotalItems;
            form.GrandTotal = vm.GrandTotal;

            if (!form.Lines.Any())
            {
                TempData["Msg"] = "Your cart is empty. Please add items first.";
                return RedirectToAction("Index", "Catalog");
            }

            if (!ModelState.IsValid)
            {
                return View(form);
            }

            // Create a real Order + Lines (Channel=Online)
            var order = new Order
            {
                Channel = "Online",
                CustomerName = form.CustomerName,
                CustomerEmail = form.Email,
                Phone = form.Phone,
                Total = form.GrandTotal,
                Lines = form.Lines.Select(l => new OrderLine
                {
                    ProductId = l.ProductId,
                    Name = l.Name,
                    Price = l.Price,
                    Qty = l.Qty
                }).ToList()
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            _cart.Clear();
            return RedirectToAction(nameof(Success));
        }

        // GET: /Storefront/Checkout/Success
        public IActionResult Success()
        {
            return View();
        }

        // helper: map cart -> VM using DB products
        private async Task<CheckoutVM> BuildVmFromCartAsync()
        {
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

                return new CartRowVM
                {
                    ProductId = id,
                    Name = p?.Name ?? "Unknown",
                    Price = p?.Price ?? 0m,
                    Qty = qty
                };
            }).ToList();

            return new CheckoutVM
            {
                Lines = lines,
                TotalItems = lines.Sum(x => x.Qty),
                GrandTotal = lines.Sum(x => x.LineTotal)
            };
        }
    }
}


