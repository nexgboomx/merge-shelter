using MergeShelter.Economy;
using MergeShelter.Meta;
using NUnit.Framework;

namespace MergeShelter.Tests.EditMode
{
    public sealed class SessionProgressionStateTests
    {
        [Test]
        public void NewPlayer_StartsAtLevelOne()
        {
            var progression = new SessionProgressionState();

            Assert.AreEqual(1, progression.HighestUnlockedLevel);
            Assert.AreEqual(1, progression.SelectedLevel);
            Assert.AreEqual(1, progression.CurrentLevel);
            Assert.AreEqual(0, progression.Coins);
            Assert.AreEqual(0, progression.Parts);
            Assert.AreEqual(1, progression.ShelterUpgradeLevel);
            Assert.IsFalse(progression.HasPendingReward);
        }

        [Test]
        public void TrySelectLevel_RejectsLockedLevel()
        {
            var progression = new SessionProgressionState();

            var selected = progression.TrySelectLevel(2);

            Assert.IsFalse(selected);
            Assert.AreEqual(1, progression.SelectedLevel);
        }

        [Test]
        public void TrySelectLevel_AllowsUnlockedLevel()
        {
            var progression = new SessionProgressionState();

            progression.UnlockThroughLevel(3);
            var selected = progression.TrySelectLevel(3);

            Assert.IsTrue(selected);
            Assert.AreEqual(3, progression.SelectedLevel);
            Assert.AreEqual(3, progression.CurrentLevel);
        }

        [Test]
        public void Currency_CanBeAddedAndSpentSafely()
        {
            var progression = new SessionProgressionState();

            progression.AddCurrency(CurrencyType.Coins, 150);
            progression.AddCurrency(CurrencyType.Parts, 4);

            Assert.AreEqual(150, progression.Coins);
            Assert.AreEqual(4, progression.Parts);
            Assert.IsTrue(progression.TrySpendCurrency(CurrencyType.Coins, 100));
            Assert.IsTrue(progression.TrySpendCurrency(CurrencyType.Parts, 2));
            Assert.AreEqual(50, progression.Coins);
            Assert.AreEqual(2, progression.Parts);
        }

        [Test]
        public void Currency_RejectsInvalidOrInsufficientSpend()
        {
            var progression = new SessionProgressionState();

            progression.AddCurrency(CurrencyType.Coins, -25);
            Assert.AreEqual(0, progression.Coins);

            progression.AddCurrency(CurrencyType.Coins, 50);

            Assert.IsFalse(progression.TrySpendCurrency(CurrencyType.Coins, 0));
            Assert.IsFalse(progression.TrySpendCurrency(CurrencyType.Coins, -1));
            Assert.IsFalse(progression.TrySpendCurrency(CurrencyType.Coins, 75));
            Assert.AreEqual(50, progression.Coins);
        }

        [Test]
        public void ShelterUpgradeLevel_TracksSuccessfulUpgrade()
        {
            var progression = new SessionProgressionState();
            progression.AddCurrency(CurrencyType.Coins, progression.ShelterUpgradeCost);

            var upgraded = progression.TryUpgradeShelter();

            Assert.IsTrue(upgraded);
            Assert.AreEqual(2, progression.ShelterUpgradeLevel);
            Assert.AreEqual(0, progression.Coins);
        }

        [Test]
        public void ShelterUpgrade_BlockedWhenCoinsAreInsufficient()
        {
            var progression = new SessionProgressionState();
            var startingCoins = progression.ShelterUpgradeCost - 1;
            progression.AddCurrency(CurrencyType.Coins, startingCoins);

            var upgraded = progression.TryUpgradeShelter();

            Assert.IsFalse(upgraded);
            Assert.AreEqual(1, progression.ShelterUpgradeLevel);
            Assert.AreEqual(startingCoins, progression.Coins);
            Assert.IsFalse(progression.CanAffordShelterUpgrade);
        }

        [Test]
        public void ShelterUpgrade_SpendsCoinsAndIncreasesCost()
        {
            var progression = new SessionProgressionState();
            var firstCost = progression.ShelterUpgradeCost;
            progression.AddCurrency(CurrencyType.Coins, firstCost + 200);

            var upgraded = progression.TryUpgradeShelter();

            Assert.IsTrue(upgraded);
            Assert.AreEqual(2, progression.ShelterUpgradeLevel);
            Assert.AreEqual(200, progression.Coins);
            Assert.Greater(progression.ShelterUpgradeCost, firstCost);
        }

        [Test]
        public void ShelterUpgrade_IncreasesMaxHealth()
        {
            var progression = new SessionProgressionState();
            var baseMaxHealth = progression.GetShelterMaxHealth(100);
            progression.AddCurrency(CurrencyType.Coins, progression.ShelterUpgradeCost);

            progression.TryUpgradeShelter();

            Assert.Greater(progression.GetShelterMaxHealth(100), baseMaxHealth);
            Assert.AreEqual(25, progression.ShelterMaxHealthBonus);
        }

        [Test]
        public void PendingReward_CanBeStoredAndCleared()
        {
            var progression = new SessionProgressionState();

            var stored = progression.TryStorePendingReward(1, 50, 1);

            Assert.IsTrue(stored);
            Assert.IsTrue(progression.HasPendingReward);
            Assert.AreEqual(1, progression.PendingReward.LevelId);
            Assert.AreEqual(50, progression.PendingReward.Coins);
            Assert.AreEqual(1, progression.PendingReward.Parts);
            Assert.IsTrue(progression.TryClearPendingReward());
            Assert.IsFalse(progression.HasPendingReward);
        }

        [Test]
        public void PendingReward_CannotBeOverwrittenBeforeClear()
        {
            var progression = new SessionProgressionState();

            Assert.IsTrue(progression.TryStorePendingReward(1, 50, 0));

            var storedSecondReward = progression.TryStorePendingReward(1, 100, 1);

            Assert.IsFalse(storedSecondReward);
            Assert.AreEqual(50, progression.PendingReward.Coins);
            Assert.AreEqual(0, progression.PendingReward.Parts);
        }

        [Test]
        public void TryClaimPendingReward_AddsCurrencyClearsRewardAndUnlocksNextLevel()
        {
            var progression = new SessionProgressionState();
            progression.TryStorePendingReward(1, 70, 2);

            var claimed = progression.TryClaimPendingReward(out var reward);

            Assert.IsTrue(claimed);
            Assert.AreEqual(1, reward.LevelId);
            Assert.AreEqual(70, progression.Coins);
            Assert.AreEqual(2, progression.Parts);
            Assert.AreEqual(2, progression.HighestUnlockedLevel);
            Assert.IsFalse(progression.HasPendingReward);
        }

        [Test]
        public void TryClaimPendingReward_CannotClaimTwice()
        {
            var progression = new SessionProgressionState();
            progression.TryStorePendingReward(1, 70, 2);
            progression.TryClaimPendingReward(out _);

            var claimedAgain = progression.TryClaimPendingReward(out var reward);

            Assert.IsFalse(claimedAgain);
            Assert.IsTrue(reward.IsEmpty);
            Assert.AreEqual(70, progression.Coins);
            Assert.AreEqual(2, progression.Parts);
            Assert.AreEqual(2, progression.HighestUnlockedLevel);
        }

        [Test]
        public void TryDoublePendingReward_DoublesCoinsAndParts()
        {
            var progression = new SessionProgressionState();
            progression.TryStorePendingReward(1, 70, 2);

            var doubled = progression.TryDoublePendingReward(out var reward);

            Assert.IsTrue(doubled);
            Assert.AreEqual(1, reward.LevelId);
            Assert.AreEqual(140, reward.Coins);
            Assert.AreEqual(4, reward.Parts);
            Assert.AreEqual(140, progression.PendingReward.Coins);
            Assert.AreEqual(4, progression.PendingReward.Parts);
        }

        [Test]
        public void TryDoublePendingReward_BlockedWithoutPendingReward()
        {
            var progression = new SessionProgressionState();

            var doubled = progression.TryDoublePendingReward(out var reward);

            Assert.IsFalse(doubled);
            Assert.IsTrue(reward.IsEmpty);
        }
    }
}
