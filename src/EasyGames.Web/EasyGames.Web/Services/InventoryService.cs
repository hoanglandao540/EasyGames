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
                row = new ShopStock { ShopId = shopId, ProductId = productId, Quantity = 0, ReorderLevel = 0 };
                _db.ShopStocks.Add(row);
                await _db.SaveChangesAsync();
            }
        }

        // Read current quantity (0 if no row).
        public async Task<int> GetQtyAsync(int shopId, int productId)
        {
            var row = await _db.ShopStocks.AsNoTracking().FirstOrDefaultAsync(s => s.ShopId == shopId && s.ProductId == productId);
            return row?.Quantity ?? 0;
        }

        // Change only the reorder level (no qty change).
        public async Task SetReorderLevelAsync(int shopId, int productId, int level)
        {
            await EnsureRowAsync(shopId, productId);
            var row = await _db.ShopStocks.FirstAsync(s => s.ShopId == shopId && s.ProductId == productId);
            row.ReorderLevel = level < 0 ? 0 : level;
            await _db.SaveChangesAsync();
        }

        // Increase qty (never goes negative).
        public async Task<bool> IncreaseAsync(int shopId, int productId, int qty)
        {
            if (qty <= 0) return true;
            await EnsureRowAsync(shopId, productId);
            var row = await _db.ShopStocks.FirstAsync(s => s.ShopId == shopId && s.ProductId == productId);
            row.Quantity += qty;
            await _db.SaveChangesAsync();
            return true;
        }

        // Decrease qty; can oversell if allowOversell=true.
        public async Task<bool> DecreaseAsync(int shopId, int productId, int qty, bool allowOversell=false)
        {
            if (qty <= 0) return true;
            await EnsureRowAsync(shopId, productId);
            var row = await _db.ShopStocks.FirstAsync(s => s.ShopId == shopId && s.ProductId == productId);

            var newQty = row.Quantity - qty;
            if (!allowOversell && newQty < 0) return false; // reject if not allowed
            row.Quantity = newQty;                           // can go negative
            await _db.SaveChangesAsync();
            return true;
        }

        // Move stock between shops (source cannot go negative).
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


