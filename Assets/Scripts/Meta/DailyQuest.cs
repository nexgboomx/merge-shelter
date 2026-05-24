using System;
using System.Collections.Generic;
using MergeShelter.Economy;

namespace MergeShelter.Meta
{
    public readonly struct DailyQuestState
    {
        public string QuestId { get; }
        public string Title { get; }
        public int Target { get; }
        public int Progress { get; }
        public bool Completed { get; }
        public bool Claimed { get; }
        public int RewardCoins { get; }
        public int RewardParts { get; }
        public bool IsEmpty => string.IsNullOrEmpty(QuestId);
        public bool CanClaim => Completed && !Claimed;

        public DailyQuestState(
            string questId,
            string title,
            int target,
            int progress,
            bool completed,
            bool claimed,
            int rewardCoins,
            int rewardParts)
        {
            QuestId = questId;
            Title = title;
            Target = target;
            Progress = progress;
            Completed = completed;
            Claimed = claimed;
            RewardCoins = rewardCoins;
            RewardParts = rewardParts;
        }
    }

    public readonly struct DailyQuestProgressResult
    {
        public string QuestId { get; }
        public string Title { get; }
        public int Target { get; }
        public int Progress { get; }
        public bool Completed { get; }
        public bool NewlyCompleted { get; }
        public int RewardCoins { get; }
        public int RewardParts { get; }
        public bool IsEmpty => string.IsNullOrEmpty(QuestId);

        public DailyQuestProgressResult(
            string questId,
            string title,
            int target,
            int progress,
            bool completed,
            bool newlyCompleted,
            int rewardCoins,
            int rewardParts)
        {
            QuestId = questId;
            Title = title;
            Target = target;
            Progress = progress;
            Completed = completed;
            NewlyCompleted = newlyCompleted;
            RewardCoins = rewardCoins;
            RewardParts = rewardParts;
        }
    }

    public readonly struct DailyQuestClaimResult
    {
        public string QuestId { get; }
        public string Title { get; }
        public int Coins { get; }
        public int Parts { get; }
        public bool IsEmpty => string.IsNullOrEmpty(QuestId);

        public DailyQuestClaimResult(string questId, string title, int coins, int parts)
        {
            QuestId = questId;
            Title = title;
            Coins = coins;
            Parts = parts;
        }
    }

    public sealed class DailyQuestModel
    {
        public const string PlaceTilesQuestId = "place_10_tiles";
        public const string CompleteLevelQuestId = "complete_1_level";
        public const string ClaimRewardQuestId = "claim_1_reward";

        private readonly List<DailyQuest> _quests;

        public DailyQuestModel(IEnumerable<DailyQuestState> questStates = null)
        {
            _quests = new List<DailyQuest>();
            if (questStates == null)
            {
                AddDefaultQuests();
                return;
            }

            foreach (var state in questStates)
                _quests.Add(new DailyQuest(state));
        }

        public bool HasClaimableQuest
        {
            get
            {
                foreach (var quest in _quests)
                {
                    if (quest.CanClaim)
                        return true;
                }

                return false;
            }
        }

        public IReadOnlyList<DailyQuestState> GetQuestStates()
        {
            var states = new List<DailyQuestState>(_quests.Count);
            foreach (var quest in _quests)
                states.Add(quest.ToState());

            return states;
        }

        public bool TryAddProgress(string questId, int amount, out DailyQuestProgressResult result)
        {
            result = default;
            if (amount <= 0)
                return false;

            var quest = FindQuest(questId);
            if (quest == null || quest.Completed)
                return false;

            result = quest.AddProgress(amount);
            return true;
        }

        public bool TryClaimFirstCompleted(CurrencyWallet wallet, out DailyQuestClaimResult result)
        {
            if (wallet == null)
                throw new ArgumentNullException(nameof(wallet));

            result = default;
            foreach (var quest in _quests)
            {
                if (!quest.CanClaim)
                    continue;

                quest.Claimed = true;
                wallet.Add(CurrencyType.Coins, quest.RewardCoins);
                wallet.Add(CurrencyType.Parts, quest.RewardParts);
                result = new DailyQuestClaimResult(quest.QuestId, quest.Title, quest.RewardCoins, quest.RewardParts);
                return true;
            }

            return false;
        }

        private void AddDefaultQuests()
        {
            _quests.Add(new DailyQuest(PlaceTilesQuestId, "Place 10 Tiles", 10, 40, 0));
            _quests.Add(new DailyQuest(CompleteLevelQuestId, "Complete 1 Level", 1, 60, 1));
            _quests.Add(new DailyQuest(ClaimRewardQuestId, "Claim 1 Reward", 1, 50, 1));
        }

        private DailyQuest FindQuest(string questId)
        {
            foreach (var quest in _quests)
            {
                if (quest.QuestId == questId)
                    return quest;
            }

            return null;
        }

        private sealed class DailyQuest
        {
            public string QuestId { get; }
            public string Title { get; }
            public int Target { get; }
            public int Progress { get; private set; }
            public bool Completed => Progress >= Target;
            public bool Claimed { get; set; }
            public int RewardCoins { get; }
            public int RewardParts { get; }
            public bool CanClaim => Completed && !Claimed;

            public DailyQuest(string questId, string title, int target, int rewardCoins, int rewardParts)
            {
                if (string.IsNullOrWhiteSpace(questId))
                    throw new ArgumentException("Quest id is required.", nameof(questId));

                if (string.IsNullOrWhiteSpace(title))
                    throw new ArgumentException("Quest title is required.", nameof(title));

                if (target <= 0)
                    throw new ArgumentOutOfRangeException(nameof(target));

                if (rewardCoins < 0)
                    throw new ArgumentOutOfRangeException(nameof(rewardCoins));

                if (rewardParts < 0)
                    throw new ArgumentOutOfRangeException(nameof(rewardParts));

                QuestId = questId;
                Title = title;
                Target = target;
                RewardCoins = rewardCoins;
                RewardParts = rewardParts;
            }

            public DailyQuest(DailyQuestState state)
                : this(state.QuestId, state.Title, state.Target, state.RewardCoins, state.RewardParts)
            {
                Progress = Math.Min(Target, Math.Max(0, state.Progress));
                Claimed = state.Claimed;
            }

            public DailyQuestProgressResult AddProgress(int amount)
            {
                var wasCompleted = Completed;
                Progress = Math.Min(Target, Progress + amount);
                var isCompleted = Completed;
                return new DailyQuestProgressResult(
                    QuestId,
                    Title,
                    Target,
                    Progress,
                    isCompleted,
                    !wasCompleted && isCompleted,
                    RewardCoins,
                    RewardParts);
            }

            public DailyQuestState ToState()
            {
                return new DailyQuestState(
                    QuestId,
                    Title,
                    Target,
                    Progress,
                    Completed,
                    Claimed,
                    RewardCoins,
                    RewardParts);
            }
        }
    }
}
