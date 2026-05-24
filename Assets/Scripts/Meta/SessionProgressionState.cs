using System;
using System.Collections.Generic;
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
        private readonly DailyReward _dailyReward;
        private readonly DailyQuestModel _dailyQuests;
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
        public bool CanClaimDailyReward => _dailyReward.CanClaim;
        public bool HasClaimedDailyReward => _dailyReward.HasClaimed;
        public int DailyRewardCoins => _dailyReward.CoinReward;
        public int DailyRewardParts => _dailyReward.PartsReward;
        public bool HasClaimableDailyQuest => _dailyQuests.HasClaimableQuest;
        public IReadOnlyList<DailyQuestState> DailyQuests => _dailyQuests.GetQuestStates();

        public SessionProgressionState(
            CurrencyWallet wallet = null,
            ShelterUpgrade shelterUpgrade = null,
            DailyReward dailyReward = null,
            DailyQuestModel dailyQuests = null)
        {
            _wallet = wallet ?? new CurrencyWallet();
            _shelterUpgrade = shelterUpgrade ?? new ShelterUpgrade();
            _dailyReward = dailyReward ?? new DailyReward();
            _dailyQuests = dailyQuests ?? new DailyQuestModel();
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

        public bool TryDoublePendingReward(out PendingLevelReward reward)
        {
            reward = default;
            if (!HasPendingReward)
                return false;

            _pendingReward = new PendingLevelReward(
                _pendingReward.LevelId,
                _pendingReward.Coins * 2,
                _pendingReward.Parts * 2);
            reward = _pendingReward;
            return true;
        }

        public bool TryClaimDailyReward(out DailyRewardClaim reward)
        {
            return _dailyReward.TryClaim(_wallet, out reward);
        }

        public bool TryAddDailyQuestProgress(string questId, int amount, out DailyQuestProgressResult result)
        {
            return _dailyQuests.TryAddProgress(questId, amount, out result);
        }

        public bool TryClaimDailyQuest(out DailyQuestClaimResult result)
        {
            return _dailyQuests.TryClaimFirstCompleted(_wallet, out result);
        }
    }
}
