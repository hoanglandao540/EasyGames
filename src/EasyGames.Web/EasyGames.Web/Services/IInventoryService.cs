using System.Threading.Tasks;

namespace EasyGames.Web.Services
{
    // Small interface so our controllers or tests can depend on it.
    public interface IInventoryService
    {
        Task<int> GetQtyAsync(int shopId, int productId);
        Task<bool> EnsureRowAsync(int shopId, int productId);
        Task<bool> IncreaseAsync(int shopId, int productId, int byQty);
        Task<bool> DecreaseAsync(int shopId, int productId, int byQty);
        Task<bool> SetReorderLevelAsync(int shopId, int productId, int newLevel);
    }
}



