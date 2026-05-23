using System;
using MergeShelter.Economy;

namespace MergeShelter.Meta
{
    public readonly struct PendingLevelReward
    {
        public int LevelId { get; }
        public int Coins { get; }
        public int Parts { get; }
        public bool IsEmpty => LevelId <= 0;

        public PendingLevelReward(int levelId, int coins, int parts)
        {
            if (levelId < SessionProgressionState.FirstLevel)
                throw new ArgumentOutOfRangeException(nameof(levelId));

            if (coins < 0)
                throw new ArgumentOutOfRangeException(nameof(coins));

            if (parts < 0)
                throw new ArgumentOutOfRangeException(nameof(parts));

            LevelId = levelId;
            Coins = coins;
            Parts = parts;
        }
    }

    public sealed class SessionProgressionState
    {
        public const int FirstLevel = 1;

        private readonly CurrencyWallet _wallet;
        private readonly ShelterUpgrade _shelterUpgrade;
        private PendingLevelReward _pendingReward;

        public int HighestUnlockedLevel { get; private set; }
        public int SelectedLevel { get; private set; }
        public int CurrentLevel => SelectedLevel;
        public int Coins => _wallet.Get(CurrencyType.Coins);
        public int Parts => _wallet.Get(CurrencyType.Parts);
        public int ShelterUpgradeLevel => _shelterUpgrade.Level;
        public int ShelterUpgradeCost => _shelterUpgrade.GetUpgradeCost();
        public int ShelterMaxHealthBonus => _shelterUpgrade.GetMaxHealthBonus();
        public bool CanAffordShelterUpgrade => Coins >= ShelterUpgradeCost;
        public bool HasPendingReward => !_pendingReward.IsEmpty;
        public PendingLevelReward PendingReward => _pendingReward;

        public SessionProgressionState(CurrencyWallet wallet = null, ShelterUpgrade shelterUpgrade = null)
        {
            _wallet = wallet ?? new CurrencyWallet();
            _shelterUpgrade = shelterUpgrade ?? new ShelterUpgrade();
            HighestUnlockedLevel = FirstLevel;
            SelectedLevel = FirstLevel;
            _pendingReward = default;
        }

        public bool TrySelectLevel(int level)
        {
            if (level < FirstLevel || level > HighestUnlockedLevel)
                return false;

            SelectedLevel = level;
            return true;
        }

        public bool UnlockThroughLevel(int level)
        {
            if (level < FirstLevel || level <= HighestUnlockedLevel)
                return false;

            HighestUnlockedLevel = level;
            return true;
        }

        public bool UnlockNextLevel()
        {
            return UnlockThroughLevel(HighestUnlockedLevel + 1);
        }

        public void AddCurrency(CurrencyType type, int amount)
        {
            _wallet.Add(type, amount);
        }

        public bool TrySpendCurrency(CurrencyType type, int amount)
        {
            return _wallet.TrySpend(type, amount);
        }

        public bool TryUpgradeShelter()
        {
            return _shelterUpgrade.TryUpgrade(_wallet);
        }

        public int GetShelterMaxHealth(int baseMaxHealth)
        {
            return _shelterUpgrade.GetMaxHealth(baseMaxHealth);
        }

        public bool TryStorePendingReward(int levelId, int coins, int parts)
        {
            if (HasPendingReward || levelId < FirstLevel || levelId > HighestUnlockedLevel || coins < 0 || parts < 0)
                return false;

            _pendingReward = new PendingLevelReward(levelId, coins, parts);
            return true;
        }

        public bool TryClearPendingReward()
        {
            if (!HasPendingReward)
                return false;

            _pendingReward = default;
            return true;
        }

        public bool TryClaimPendingReward(out PendingLevelReward reward)
        {
            reward = _pendingReward;
            if (!HasPendingReward)
                return false;

            AddCurrency(CurrencyType.Coins, reward.Coins);
            AddCurrency(CurrencyType.Parts, reward.Parts);
            UnlockThroughLevel(reward.LevelId + 1);
            _pendingReward = default;
            return true;
        }
    }
}
