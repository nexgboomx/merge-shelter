using System;
using MergeShelter.Meta;

namespace MergeShelter.Save
{
    [Serializable]
    public sealed class DailyQuestSaveData
    {
        public string QuestId;
        public string Title;
        public int Target;
        public int Progress;
        public bool Completed;
        public bool Claimed;
        public int RewardCoins;
        public int RewardParts;

        public static DailyQuestSaveData FromState(DailyQuestState state)
        {
            return new DailyQuestSaveData
            {
                QuestId = state.QuestId,
                Title = state.Title,
                Target = state.Target,
                Progress = state.Progress,
                Completed = state.Completed,
                Claimed = state.Claimed,
                RewardCoins = state.RewardCoins,
                RewardParts = state.RewardParts
            };
        }

        public DailyQuestState ToState()
        {
            var progress = Completed && Progress < Target ? Target : Progress;
            return new DailyQuestState(
                QuestId,
                Title,
                Target,
                progress,
                Completed,
                Claimed,
                RewardCoins,
                RewardParts);
        }
    }
}
