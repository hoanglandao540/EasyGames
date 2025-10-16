using System.Linq;
using System.Threading.Tasks;
using EasyGames.Web.Data;
using EasyGames.Web.Models;
using EasyGames.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Web.Areas.Owner.Controllers
{
    [Area("Owner")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _db;
        public ProductsController(AppDbContext db) { _db = db; }

        // GET: /Owner/Products
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var list = await _db.Products.AsNoTracking()
                .OrderBy(p => p.Id)
                .Select(p => new ProductVM
                {
                    Id = p.Id,
                    Sku = p.Sku,
                    Name = p.Name,
                    Price = p.Price,
                    IsActive = p.IsActive
                })
                .ToListAsync();

            return View(list);
        }

        // GET: /Owner/Products/Create
        [HttpGet]
        public IActionResult Create() => View(new ProductVM());

        // POST: /Owner/Products/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var entity = new Product
            {
                Sku = vm.Sku,
                Name = vm.Name,
                Price = vm.Price,
                IsActive = vm.IsActive
            };
            _db.Products.Add(entity);
            await _db.SaveChangesAsync();
            TempData["msg"] = "Product created.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Owner/Products/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();

            var vm = new ProductVM
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name,
                Price = p.Price,
                IsActive = p.IsActive
            };
            return View(vm);
        }

        // POST: /Owner/Products/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var p = await _db.Products.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if (p == null) return NotFound();

            p.Sku = vm.Sku;
            p.Name = vm.Name;
            p.Price = vm.Price;
            p.IsActive = vm.IsActive;

            await _db.SaveChangesAsync();
            TempData["msg"] = "Product updated.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Owner/Products/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (p != null)
            {
                _db.Products.Remove(p);
                await _db.SaveChangesAsync();
                TempData["msg"] = "Product deleted.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}



