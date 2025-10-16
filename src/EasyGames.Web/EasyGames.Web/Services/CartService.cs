using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace EasyGames.Web.Services
{
    // Session-backed cart. Very small, very reliable.
    public class CartService : ICartService
    {
        private const string CartKey = "eg_cart";
        private readonly IHttpContextAccessor _http;

        public CartService(IHttpContextAccessor http)
        {
            _http = http;
        }

        private ISession Session => _http.HttpContext!.Session;

        private IDictionary<int, int> Load()
        {
            var json = Session.GetString(CartKey);
            return string.IsNullOrEmpty(json)
                ? new Dictionary<int, int>()
                : JsonSerializer.Deserialize<Dictionary<int, int>>(json) ?? new Dictionary<int, int>();
        }

        private void Save(IDictionary<int, int> map)
        {
            var json = JsonSerializer.Serialize(map);
            Session.SetString(CartKey, json);
        }

        public IDictionary<int, int> Get() => new Dictionary<int, int>(Load());

        public void Add(int productId, int byQty = 1)
        {
            if (byQty <= 0) return; // friendly guard
            var map = Load();
            map.TryGetValue(productId, out var q);
            checked { map[productId] = q + byQty; }   // checked guards overflow (rare)
            Save(map);
        }

        public void Remove(int productId, int byQty = 1)
        {
            if (byQty <= 0) return;
            var map = Load();
            if (!map.TryGetValue(productId, out var q)) return;
            q -= byQty;
            if (q <= 0) map.Remove(productId);
            else map[productId] = q;
            Save(map);
        }

        public void Clear() => Session.Remove(CartKey);

        public int TotalItems()
        {
            var map = Load();
            var total = 0;
            foreach (var kv in map) total += kv.Value;
            return total;
        }
    }
}



