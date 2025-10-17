using System.Threading.Tasks;

namespace EasyGames.Web.Services
{
    // inventory API
    public interface IInventoryService
    {
        // helpers
        Task EnsureRowAsync(int shopId, int productId);           // create row if missing (qty=0)
        Task<int> GetQtyAsync(int shopId, int productId);         // read qty
        Task SetReorderLevelAsync(int shopId, int productId, int level);

        // movements
        Task<bool> IncreaseAsync(int shopId, int productId, int qty);                    // +qty (never negative)
        Task<bool> DecreaseAsync(int shopId, int productId, int qty, bool allowOversell=false); // -qty (may go <0 if allowOversell)
        Task<bool> TransferAsync(int fromShopId, int toShopId, int productId, int qty);   // move qty between shops
    }
}


