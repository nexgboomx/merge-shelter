using MergeShelter.Economy;
using MergeShelter.Meta;
using NUnit.Framework;

namespace MergeShelter.Tests.EditMode
{
    public sealed class DailyQuestTests
    {
        [Test]
        public void PlaceTileQuest_CompletesAtTen()
        {
            var quests = new DailyQuestModel();

            Assert.IsTrue(quests.TryAddProgress(DailyQuestModel.PlaceTilesQuestId, 9, out var progress));
            Assert.AreEqual(9, progress.Progress);
            Assert.IsFalse(progress.Completed);
            Assert.IsFalse(progress.NewlyCompleted);

            Assert.IsTrue(quests.TryAddProgress(DailyQuestModel.PlaceTilesQuestId, 1, out progress));
            Assert.AreEqual(10, progress.Progress);
            Assert.IsTrue(progress.Completed);
            Assert.IsTrue(progress.NewlyCompleted);
        }

        [Test]
        public void CompleteLevelQuest_CompletesAtOne()
        {
            var quests = new DailyQuestModel();

            var progressed = quests.TryAddProgress(DailyQuestModel.CompleteLevelQuestId, 1, out var progress);

            Assert.IsTrue(progressed);
            Assert.AreEqual(1, progress.Progress);
            Assert.IsTrue(progress.Completed);
            Assert.IsTrue(progress.NewlyCompleted);
        }

        [Test]
        public void ClaimRewardQuest_CompletesAtOne()
        {
            var quests = new DailyQuestModel();

            var progressed = quests.TryAddProgress(DailyQuestModel.ClaimRewardQuestId, 1, out var progress);

            Assert.IsTrue(progressed);
            Assert.AreEqual(1, progress.Progress);
            Assert.IsTrue(progress.Completed);
            Assert.IsTrue(progress.NewlyCompleted);
        }

        [Test]
        public void CompletedQuest_GrantsRewardOnce()
        {
            var wallet = new CurrencyWallet();
            var quests = new DailyQuestModel();
            quests.TryAddProgress(DailyQuestModel.PlaceTilesQuestId, 10, out var progress);

            var claimed = quests.TryClaimFirstCompleted(wallet, out var reward);

            Assert.IsTrue(claimed);
            Assert.AreEqual(DailyQuestModel.PlaceTilesQuestId, reward.QuestId);
            Assert.AreEqual(progress.RewardCoins, wallet.Get(CurrencyType.Coins));
            Assert.AreEqual(progress.RewardParts, wallet.Get(CurrencyType.Parts));

            var claimedAgain = quests.TryClaimFirstCompleted(wallet, out var secondReward);

            Assert.IsFalse(claimedAgain);
            Assert.IsTrue(secondReward.IsEmpty);
            Assert.AreEqual(progress.RewardCoins, wallet.Get(CurrencyType.Coins));
            Assert.AreEqual(progress.RewardParts, wallet.Get(CurrencyType.Parts));
        }

        [Test]
        public void UncompletedQuest_CannotBeClaimed()
        {
            var wallet = new CurrencyWallet();
            var quests = new DailyQuestModel();
            quests.TryAddProgress(DailyQuestModel.PlaceTilesQuestId, 9, out _);

            var claimed = quests.TryClaimFirstCompleted(wallet, out var reward);

            Assert.IsFalse(claimed);
            Assert.IsTrue(reward.IsEmpty);
            Assert.AreEqual(0, wallet.Get(CurrencyType.Coins));
            Assert.AreEqual(0, wallet.Get(CurrencyType.Parts));
        }
    }
}
