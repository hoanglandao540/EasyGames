using System;
using System.Threading.Tasks;
using EasyGames.Web.Data;
using EasyGames.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Web.Services
{
    // Inventory logic with simple guard rails:
    // - never go negative
    // - auto-create a row if missing (optional)
    // - async EF calls so UI stays responsive
    public class InventoryService : IInventoryService
    {
        private readonly AppDbContext _db;

        public InventoryService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<int> GetQtyAsync(int shopId, int productId)
        {
            var row = await _db.ShopStocks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ShopId == shopId && x.ProductId == productId);

            return row?.Quantity ?? 0;
        }

        public async Task<bool> EnsureRowAsync(int shopId, int productId)
        {
            var exists = await _db.ShopStocks.AnyAsync(x => x.ShopId == shopId && x.ProductId == productId);
            if (exists) return true;

            var newRow = new ShopStock
            {
                ShopId = shopId,
                ProductId = productId,
                Quantity = 0,
                ReorderLevel = 0
            };

            _db.ShopStocks.Add(newRow);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IncreaseAsync(int shopId, int productId, int byQty)
        {
            if (byQty <= 0) return false; // human rule: must be positive

            var row = await _db.ShopStocks.FirstOrDefaultAsync(x => x.ShopId == shopId && x.ProductId == productId);
            if (row == null)
            {
                // If missing, create it with starting qty.
                row = new ShopStock { ShopId = shopId, ProductId = productId, Quantity = byQty };
                _db.ShopStocks.Add(row);
            }
            else
            {
                checked
                {
                    row.Quantity += byQty; // checked to detect overflow (rare, but tidy)
                }
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DecreaseAsync(int shopId, int productId, int byQty)
        {
            if (byQty <= 0) return false;

            var row = await _db.ShopStocks.FirstOrDefaultAsync(x => x.ShopId == shopId && x.ProductId == productId);
            if (row == null) return false;

            if (row.Quantity - byQty < 0) return false; // never negative

            row.Quantity -= byQty;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetReorderLevelAsync(int shopId, int productId, int newLevel)
        {
            if (newLevel < 0) return false;

            var row = await _db.ShopStocks.FirstOrDefaultAsync(x => x.ShopId == shopId && x.ProductId == productId);
            if (row == null) return false;

            row.ReorderLevel = newLevel;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}



