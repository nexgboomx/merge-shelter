using System;
using System.IO;
using MergeShelter.Meta;
using MergeShelter.Save;
using NUnit.Framework;

namespace MergeShelter.Tests.EditMode
{
    public sealed class LocalJsonSaveServiceTests
    {
        private string _tempDirectory;

        [SetUp]
        public void SetUp()
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), "MergeShelterSaveTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_tempDirectory))
                Directory.Delete(_tempDirectory, true);
        }

        [Test]
        public void SaveThenLoad_ReturnsSameData()
        {
            var service = new LocalJsonSaveService(_tempDirectory);
            var expected = CreateSaveData();

            service.Save(expected);

            Assert.IsTrue(service.HasSave());
            Assert.IsTrue(service.TryLoad(out var loaded));
            AssertSaveDataEqual(expected, loaded);
        }

        [Test]
        public void TryLoad_MissingSaveReturnsFalse()
        {
            var service = new LocalJsonSaveService(_tempDirectory);

            var loaded = service.TryLoad(out var saveData);

            Assert.IsFalse(loaded);
            Assert.IsNull(saveData);
            Assert.IsFalse(service.HasSave());
        }

        [Test]
        public void DeleteAndReset_RemoveSave()
        {
            var service = new LocalJsonSaveService(_tempDirectory);
            service.Save(CreateSaveData());

            service.Delete();

            Assert.IsFalse(service.HasSave());
            Assert.IsFalse(service.TryLoad(out _));

            service.Save(CreateSaveData());
            service.Reset();

            Assert.IsFalse(service.HasSave());
            Assert.IsFalse(service.TryLoad(out _));
        }

        [Test]
        public void TryLoad_CorruptJsonFailsSafely()
        {
            var service = new LocalJsonSaveService(_tempDirectory);
            File.WriteAllText(service.SaveFilePath, "not valid json");

            var loaded = service.TryLoad(out var saveData);

            Assert.IsFalse(loaded);
            Assert.IsNull(saveData);
            Assert.IsTrue(service.HasSave());
        }

        private static GameSaveData CreateSaveData()
        {
            return new GameSaveData
            {
                Coins = 175,
                Parts = 4,
                HighestUnlockedLevel = 5,
                SelectedLevel = 3,
                ShelterUpgradeLevel = 2,
                DailyRewardClaimed = true,
                DailyQuests =
                {
                    new DailyQuestSaveData
                    {
                        QuestId = DailyQuestModel.PlaceTilesQuestId,
                        Title = "Place 10 Tiles",
                        Target = 10,
                        Progress = 10,
                        Completed = true,
                        Claimed = true,
                        RewardCoins = 40,
                        RewardParts = 0
                    },
                    new DailyQuestSaveData
                    {
                        QuestId = DailyQuestModel.CompleteLevelQuestId,
                        Title = "Complete 1 Level",
                        Target = 1,
                        Progress = 1,
                        Completed = true,
                        Claimed = false,
                        RewardCoins = 60,
                        RewardParts = 1
                    },
                    new DailyQuestSaveData
                    {
                        QuestId = DailyQuestModel.ClaimRewardQuestId,
                        Title = "Claim 1 Reward",
                        Target = 1,
                        Progress = 0,
                        Completed = false,
                        Claimed = false,
                        RewardCoins = 50,
                        RewardParts = 1
                    }
                }
            };
        }

        private static void AssertSaveDataEqual(GameSaveData expected, GameSaveData actual)
        {
            Assert.NotNull(actual);
            Assert.AreEqual(expected.SaveVersion, actual.SaveVersion);
            Assert.AreEqual(expected.Coins, actual.Coins);
            Assert.AreEqual(expected.Parts, actual.Parts);
            Assert.AreEqual(expected.HighestUnlockedLevel, actual.HighestUnlockedLevel);
            Assert.AreEqual(expected.SelectedLevel, actual.SelectedLevel);
            Assert.AreEqual(expected.ShelterUpgradeLevel, actual.ShelterUpgradeLevel);
            Assert.AreEqual(expected.DailyRewardClaimed, actual.DailyRewardClaimed);
            Assert.AreEqual(expected.DailyQuests.Count, actual.DailyQuests.Count);

            for (var i = 0; i < expected.DailyQuests.Count; i++)
            {
                var expectedQuest = expected.DailyQuests[i];
                var actualQuest = actual.DailyQuests[i];
                Assert.AreEqual(expectedQuest.QuestId, actualQuest.QuestId);
                Assert.AreEqual(expectedQuest.Title, actualQuest.Title);
                Assert.AreEqual(expectedQuest.Target, actualQuest.Target);
                Assert.AreEqual(expectedQuest.Progress, actualQuest.Progress);
                Assert.AreEqual(expectedQuest.Completed, actualQuest.Completed);
                Assert.AreEqual(expectedQuest.Claimed, actualQuest.Claimed);
                Assert.AreEqual(expectedQuest.RewardCoins, actualQuest.RewardCoins);
                Assert.AreEqual(expectedQuest.RewardParts, actualQuest.RewardParts);
            }
        }
    }
}
