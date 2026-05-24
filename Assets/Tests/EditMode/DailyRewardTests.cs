using MergeShelter.Economy;
using MergeShelter.Meta;
using NUnit.Framework;

namespace MergeShelter.Tests.EditMode
{
    public sealed class DailyRewardTests
    {
        [Test]
        public void NewReward_IsAvailableAtSessionStart()
        {
            var reward = new DailyReward();

            Assert.IsTrue(reward.CanClaim);
            Assert.IsFalse(reward.HasClaimed);
            Assert.Greater(reward.CoinReward, 0);
            Assert.Greater(reward.PartsReward, 0);
        }

        [Test]
        public void TryClaim_GrantsCoinsAndParts()
        {
            var wallet = new CurrencyWallet();
            var reward = new DailyReward(25, 2);

            var claimed = reward.TryClaim(wallet, out var claim);

            Assert.IsTrue(claimed);
            Assert.AreEqual(25, claim.Coins);
            Assert.AreEqual(2, claim.Parts);
            Assert.AreEqual(25, wallet.Get(CurrencyType.Coins));
            Assert.AreEqual(2, wallet.Get(CurrencyType.Parts));
            Assert.IsTrue(reward.HasClaimed);
            Assert.IsFalse(reward.CanClaim);
        }

        [Test]
        public void TryClaim_BlocksDoubleClaim()
        {
            var wallet = new CurrencyWallet();
            var reward = new DailyReward(25, 2);

            Assert.IsTrue(reward.TryClaim(wallet, out _));

            var claimedAgain = reward.TryClaim(wallet, out var secondClaim);

            Assert.IsFalse(claimedAgain);
            Assert.IsTrue(secondClaim.IsEmpty);
            Assert.AreEqual(25, wallet.Get(CurrencyType.Coins));
            Assert.AreEqual(2, wallet.Get(CurrencyType.Parts));
        }
    }
}
