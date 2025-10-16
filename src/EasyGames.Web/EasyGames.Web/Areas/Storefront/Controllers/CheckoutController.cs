using System.Linq;
using EasyGames.Web.Services;
using EasyGames.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EasyGames.Web.Areas.Storefront.Controllers
{
    [Area("Storefront")]
    public class CheckoutController : Controller
    {
        private readonly ICartService _cart;

        public CheckoutController(ICartService cart)
        {
            _cart = cart;
        }

        // GET: /Storefront/Checkout
        public IActionResult Index()
        {
            var vm = BuildVmFromCart();
            if (!vm.Lines.Any())
            {
                TempData["Msg"] = "Your cart is empty. Please add items first.";
                return RedirectToAction("Index", "Catalog");
            }
            return View(vm);
        }

        // POST: /Storefront/Checkout
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Index(CheckoutVM form)
        {
            // Rehydrated cart lines so the page can re-render on validation errors
            var vm = BuildVmFromCart();
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
                // we simply re-show the form with validation messages.
                return View(form);
            }

            
            _cart.Clear();

            return RedirectToAction(nameof(Success));
        }

        // GET: /Storefront/Checkout/Success
        public IActionResult Success()
        {
            return View();
        }

        // helper: map cart -> VM
        private CheckoutVM BuildVmFromCart()
        {
            var map = _cart.Get(); // productId -> qty

            var lines = map.Select(kv =>
            {
                var id = kv.Key;
                var qty = kv.Value;
                ProductCatalog.TryGet(id, out var info);

                return new CartRowVM
                {
                    ProductId = id,
                    Name = info?.Name ?? "Unknown",
                    Price = info?.Price ?? 0m,
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



