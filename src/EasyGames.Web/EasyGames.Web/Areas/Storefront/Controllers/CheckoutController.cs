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

        public async Task<IActionResult> Index()
        {
            var map = _cart.Get();
            var ids = map.Keys.ToList();
            var products = await _db.Products.AsNoTracking().Where(p => ids.Contains(p.Id)).ToDictionaryAsync(p => p.Id);
            var lines = map.Select(kv => new CartRowVM {
                ProductId = kv.Key,
                Name = products.TryGetValue(kv.Key, out var p) ? p.Name : "Unknown",
                Price = products.TryGetValue(kv.Key, out var p2) ? p2.Price : 0m,
                Qty = kv.Value
            }).ToList();

            return View(new CheckoutVM { Lines = lines });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CheckoutVM form)
        {
            var map = _cart.Get();
            if (map.Count == 0)
            {
                TempData["Msg"] = "Cart is empty.";
                return RedirectToAction("Index", "Catalog");
            }

            var ids = map.Keys.ToList();
            var products = await _db.Products.AsNoTracking().Where(p => ids.Contains(p.Id)).ToDictionaryAsync(p => p.Id);
            var lines = map.Select(kv => new CartRowVM {
                ProductId = kv.Key,
                Name = products.TryGetValue(kv.Key, out var p) ? p.Name : "Unknown",
                Price = products.TryGetValue(kv.Key, out var p2) ? p2.Price : 0m,
                Qty = kv.Value
            }).ToList();

            var order = new Order
            {
                Channel = "Online",
                CustomerName = form.CustomerName,
                CustomerEmail = form.Email,
                Phone = form.Phone,
                Total = lines.Sum(x => x.LineTotal),
                Lines = lines.Select(l => new OrderLine { ProductId = l.ProductId, Name = l.Name, Price = l.Price, Qty = l.Qty }).ToList()
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            _cart.Clear();
            return RedirectToAction("Success");
        }

        public IActionResult Success() => View();
    }
}


