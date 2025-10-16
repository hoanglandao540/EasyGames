using EasyGames.Web.Models;

namespace EasyGames.Web.Services
{
    // Interface so we can unit test and swap logic later if needed
    public interface ITierService
    {
        // Returns a Tier based on how much a customer spent in total
        Tier GetTier(decimal lifetimeSpend);
    }

    // Very simple threshold rules for now.
    // Bronze: < 500
    // Silver: 500 to 999.99
    // Gold:   >= 1000
    public class TierService : ITierService
    {
        private const decimal SilverStart = 500m;
        private const decimal GoldStart = 1000m;

        public Tier GetTier(decimal lifetimeSpend)
        {
            // Make sure negative inputs do not break anything
            if (lifetimeSpend < SilverStart)
            {
                return Tier.Bronze;
            }

            if (lifetimeSpend < GoldStart)
            {
                return Tier.Silver;
            }

            return Tier.Gold;
        }
    }
}



