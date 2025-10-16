using System.Linq;
using System.Threading.Tasks;
using EasyGames.Web.Data;
using EasyGames.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Web.Areas.Owner.Controllers
{
    // async + AsNoTracking for read screens.
    [Area("Owner")]
    public class ShopsController : Controller
    {
        private readonly AppDbContext _db;

        public ShopsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /Owner/Shops
        public async Task<IActionResult> Index()
        {
            var shops = await _db.Shops
                .Include(s => s.Stocks)
                .AsNoTracking()
                .ToListAsync();

            var vms = shops.Select(s => new ShopVM
            {
                Id = s.Id,
                ShopCode = s.ShopCode,
                ShopName = s.ShopName,
                City = s.City,
                Country = s.Country,
                Phone = s.Phone,
                TotalSkus = s.Stocks.Count,
                TotalQty = s.Stocks.Sum(x => x.Quantity),
                LowStockSkus = s.Stocks.Count(x => x.Quantity <= x.ReorderLevel)
            }).ToList();

            return View(vms);
        }
    }
}



