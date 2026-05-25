using System;
using System.Collections.Generic;
using MergeShelter.Meta;

namespace MergeShelter.Save
{
    [Serializable]
    public sealed class GameSaveData
    {
        public const int CurrentSaveVersion = 1;

        public int SaveVersion = CurrentSaveVersion;
        public int Coins;
        public int Parts;
        public int HighestUnlockedLevel = SessionProgressionState.FirstLevel;
        public int SelectedLevel = SessionProgressionState.FirstLevel;
        public int ShelterUpgradeLevel = 1;
        public bool DailyRewardClaimed;
        public bool TutorialStateSaved;
        public int TutorialStep;
        public int TutorialTilesPlaced;
        public List<DailyQuestSaveData> DailyQuests = new();

        public bool IsValid()
        {
            if (SaveVersion != CurrentSaveVersion ||
                Coins < 0 ||
                Parts < 0 ||
                HighestUnlockedLevel < SessionProgressionState.FirstLevel ||
                SelectedLevel < SessionProgressionState.FirstLevel ||
                SelectedLevel > HighestUnlockedLevel ||
                ShelterUpgradeLevel < 1 ||
                TutorialStep < 0 ||
                TutorialStep > 6 ||
                TutorialTilesPlaced < 0 ||
                DailyQuests == null)
            {
                return false;
            }

            foreach (var quest in DailyQuests)
            {
                if (quest == null ||
                    string.IsNullOrWhiteSpace(quest.QuestId) ||
                    string.IsNullOrWhiteSpace(quest.Title) ||
                    quest.Target <= 0 ||
                    quest.Progress < 0 ||
                    quest.RewardCoins < 0 ||
                    quest.RewardParts < 0)
                {
                    return false;
                }
            }

            return true;
        }

        public void EnsureDefaults()
        {
            if (SaveVersion <= 0)
                SaveVersion = CurrentSaveVersion;

            if (HighestUnlockedLevel < SessionProgressionState.FirstLevel)
                HighestUnlockedLevel = SessionProgressionState.FirstLevel;

            if (SelectedLevel < SessionProgressionState.FirstLevel)
                SelectedLevel = SessionProgressionState.FirstLevel;

            if (ShelterUpgradeLevel < 1)
                ShelterUpgradeLevel = 1;

            if (TutorialStep < 0)
                TutorialStep = 0;
            else if (TutorialStep > 6)
                TutorialStep = 6;

            if (TutorialTilesPlaced < 0)
                TutorialTilesPlaced = 0;

            DailyQuests ??= new List<DailyQuestSaveData>();
        }

        public static GameSaveData FromProgression(SessionProgressionState progression)
        {
            if (progression == null)
                throw new ArgumentNullException(nameof(progression));

            var saveData = new GameSaveData
            {
                Coins = progression.Coins,
                Parts = progression.Parts,
                HighestUnlockedLevel = progression.HighestUnlockedLevel,
                SelectedLevel = progression.SelectedLevel,
                ShelterUpgradeLevel = progression.ShelterUpgradeLevel,
                DailyRewardClaimed = progression.HasClaimedDailyReward,
                DailyQuests = new List<DailyQuestSaveData>()
            };

            foreach (var quest in progression.DailyQuests)
                saveData.DailyQuests.Add(DailyQuestSaveData.FromState(quest));

            return saveData;
        }

        public IReadOnlyList<DailyQuestState> ToDailyQuestStates()
        {
            var states = new List<DailyQuestState>();
            if (DailyQuests == null)
                return states;

            foreach (var quest in DailyQuests)
                states.Add(quest.ToState());

            return states;
        }
    }
}
