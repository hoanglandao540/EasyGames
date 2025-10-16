using Xunit;
using EasyGames.Web.Services;
using EasyGames.Web.Models;

namespace EasyGames.Tests
{
    // Very basic tests for the TierService
    public class TierServiceTests
    {
        private readonly ITierService _service;

        public TierServiceTests()
        {
            _service = new TierService();
        }

        [Theory]
        // Bronze: anything below 500
        [InlineData(-10, Tier.Bronze)]
        [InlineData(0, Tier.Bronze)]
        [InlineData(499.99, Tier.Bronze)]
        public void GetTier_Returns_Bronze_For_Low_Spend(decimal spend, Tier expected)
        {
            // act
            var result = _service.GetTier(spend);

            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        // Silver: 500 to 999.99
        [InlineData(500, Tier.Silver)]
        [InlineData(750.50, Tier.Silver)]
        [InlineData(999.99, Tier.Silver)]
        public void GetTier_Returns_Silver_For_Mid_Spend(decimal spend, Tier expected)
        {
            var result = _service.GetTier(spend);
            Assert.Equal(expected, result);
        }

        [Theory]
        // Gold: 1000 and above
        [InlineData(1000, Tier.Gold)]
        [InlineData(2500.00, Tier.Gold)]
        public void GetTier_Returns_Gold_For_High_Spend(decimal spend, Tier expected)
        {
            var result = _service.GetTier(spend);
            Assert.Equal(expected, result);
        }
    }
}



