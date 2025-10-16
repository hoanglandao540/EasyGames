using System.Collections.Generic;

namespace EasyGames.Web.Services
{
    // Tiny abstraction so later we can unit-test or swap implementations
    public interface ICartService
    {
        IDictionary<int, int> Get();  // productId -> qty
        void Add(int productId, int byQty = 1); // never negative
        void Remove(int productId, int byQty = 1);   // clamps at 0
        void Clear();
        int TotalItems();      // sum of quantities
    }
}



