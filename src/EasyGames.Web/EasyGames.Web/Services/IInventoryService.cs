using System.Threading.Tasks;

namespace EasyGames.Web.Services
{
    public interface IInventoryService
    {
        Task EnsureRowAsync(int shopId, int productId);
        Task<int> GetQtyAsync(int shopId, int productId);
        Task SetReorderLevelAsync(int shopId, int productId, int level);

        Task<bool> IncreaseAsync(int shopId, int productId, int qty);
        Task<bool> DecreaseAsync(int shopId, int productId, int qty, bool allowOversell = false);
        Task<bool> TransferAsync(int fromShopId, int toShopId, int productId, int qty);
    }
}



