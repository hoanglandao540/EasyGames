using System.Linq;
using System.Threading.Tasks;
using EasyGames.Web.Data;
using EasyGames.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShopEntity = EasyGames.Web.Models.Shop;

namespace EasyGames.Web.Areas.Owner.Controllers
{
    [Area("Owner")]
    public class ShopsController : Controller
    {
        private readonly AppDbContext _db;
        public ShopsController(AppDbContext db) { _db = db; }

        // GET: /Owner/Shops
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            var shops = await _db.Shops
                .AsNoTracking()
                .Select(s => new { s.Id, s.ShopCode, s.City, s.Country, s.Phone })
                .ToListAsync();

            var stockAgg = await _db.ShopStocks
                .AsNoTracking()
                .GroupBy(ss => ss.ShopId)
                .Select(g => new
                {
                    ShopId = g.Key,
                    TotalSkus = g.Count(),
                    TotalQty = g.Sum(x => x.Quantity),
                    LowStockSkus = g.Count(x => x.Quantity <= x.ReorderLevel)
                })
                .ToListAsync();

            var vms = shops
                .Select(s =>
                {
                    var agg = stockAgg.FirstOrDefault(a => a.ShopId == s.Id);
                    var friendlyName = !string.IsNullOrWhiteSpace(s.ShopCode)
                        ? s.ShopCode
                        : $"{(s.City ?? "").Trim()}, {(s.Country ?? "").Trim()}".Trim(' ', ',');

                    return new ShopVM
                    {
                        Id = s.Id,
                        ShopCode = s.ShopCode,
                        ShopName = string.IsNullOrWhiteSpace(friendlyName) ? $"Shop #{s.Id}" : friendlyName,
                        City = s.City,
                        Country = s.Country,
                        Phone = s.Phone,
                        TotalSkus = agg?.TotalSkus ?? 0,
                        TotalQty = agg?.TotalQty ?? 0,
                        LowStockSkus = agg?.LowStockSkus ?? 0
                    };
                })
                .OrderBy(x => x.Id)
                .ToList();

            return View(vms);
        }

        // GET: /Owner/Shops/Create
        [HttpGet]
        public IActionResult Create() => View(new ShopCreateVM());

        // POST: /Owner/Shops/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShopCreateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var shopEntity = new ShopEntity
            {
                ShopCode = vm.ShopCode,
                City = vm.City,
                Country = vm.Country,
                Phone = vm.Phone
            };

            _db.Shops.Add(shopEntity);
            await _db.SaveChangesAsync();

            TempData["msg"] = "Shop created.";
            return RedirectToAction(nameof(Index));
        }
    }
}



