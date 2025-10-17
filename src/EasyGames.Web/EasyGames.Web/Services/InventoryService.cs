using System.Threading.Tasks;
using EasyGames.Web.Data;
using EasyGames.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Web.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _db;
        public InventoryService(AppDbContext db) { _db = db; }

        public async Task EnsureRowAsync(int shopId, int productId)
        {
            var row = await _db.ShopStocks.FirstOrDefaultAsync(s => s.ShopId == shopId && s.ProductId == productId);
            if (row == null)
            {
                _db.ShopStocks.Add(new ShopStock { ShopId = shopId, ProductId = productId, Quantity = 0, ReorderLevel = 0 });
                await _db.SaveChangesAsync();
            }
        }

        public async Task<int> GetQtyAsync(int shopId, int productId)
        {
            var row = await _db.ShopStocks.AsNoTracking().FirstOrDefaultAsync(s => s.ShopId == shopId && s.ProductId == productId);
            return row?.Quantity ?? 0;
        }

        public async Task SetReorderLevelAsync(int shopId, int productId, int level)
        {
            await EnsureRowAsync(shopId, productId);
            var row = await _db.ShopStocks.FirstAsync(s => s.ShopId == shopId && s.ProductId == productId);
            row.ReorderLevel = level < 0 ? 0 : level;
            await _db.SaveChangesAsync();
        }

        public async Task<bool> IncreaseAsync(int shopId, int productId, int qty)
        {
            if (qty <= 0) return true;
            await EnsureRowAsync(shopId, productId);
            var row = await _db.ShopStocks.FirstAsync(s => s.ShopId == shopId && s.ProductId == productId);
            row.Quantity += qty;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DecreaseAsync(int shopId, int productId, int qty, bool allowOversell = false)
        {
            if (qty <= 0) return true;
            await EnsureRowAsync(shopId, productId);
            var row = await _db.ShopStocks.FirstAsync(s => s.ShopId == shopId && s.ProductId == productId);

            var newQty = row.Quantity - qty;
            if (!allowOversell && newQty < 0) return false;
            row.Quantity = newQty;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TransferAsync(int fromShopId, int toShopId, int productId, int qty)
        {
            if (qty <= 0 || fromShopId == toShopId) return false;

            await EnsureRowAsync(fromShopId, productId);
            await EnsureRowAsync(toShopId, productId);

            var from = await _db.ShopStocks.FirstAsync(s => s.ShopId == fromShopId && s.ProductId == productId);
            if (from.Quantity < qty) return false;

            var to = await _db.ShopStocks.FirstAsync(s => s.ShopId == toShopId && s.ProductId == productId);
            from.Quantity -= qty;
            to.Quantity += qty;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}



