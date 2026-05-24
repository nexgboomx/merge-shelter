using System;
using System.Collections;
using System.IO;
using System.Reflection;
using MergeShelter.Board;
using MergeShelter.Core;
using MergeShelter.Meta;
using MergeShelter.Save;
using MergeShelter.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MergeShelter.Tests.PlayMode
{
    public sealed class PrototypeSprint1SmokeTests
    {
        private const string SceneName = "PrototypeSprint1";
        private string _tempSaveDirectory;

        [SetUp]
        public void SetUp()
        {
            _tempSaveDirectory = Path.Combine(Path.GetTempPath(), "MergeShelterPlayModeSaveTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempSaveDirectory);
            PrototypeGameController.SaveDirectoryOverrideForTests = _tempSaveDirectory;
        }

        [TearDown]
        public void TearDown()
        {
            PrototypeGameController.SaveDirectoryOverrideForTests = null;
            if (Directory.Exists(_tempSaveDirectory))
                Directory.Delete(_tempSaveDirectory, true);
        }

        [UnityTest]
        public IEnumerator PrototypeScene_WiresBoardAndWaveControls()
        {
            yield return LoadPrototypeScene();

            var controller = Object.FindObjectOfType<PrototypeGameController>();
            var boardView = Object.FindObjectOfType<PrototypeBoardView>();
            var canvas = Object.FindObjectOfType<Canvas>();

            Assert.NotNull(controller);
            Assert.NotNull(boardView);
            Assert.NotNull(canvas);
            Assert.NotNull(GameObject.Find("EventSystem"));

            var firstCell = GameObject.Find("Cell_0_0")?.GetComponent<Button>();
            Assert.NotNull(firstCell);

            firstCell.onClick.Invoke();
            yield return null;

            Assert.IsFalse(controller.GetTileAt(0, 0).IsEmpty);

            var startWaveButton = GameObject.Find("StartWaveButton")?.GetComponent<Button>();
            Assert.NotNull(startWaveButton);

            startWaveButton.onClick.Invoke();
            yield return null;

            Assert.IsFalse(string.IsNullOrWhiteSpace(GetResultText().text));
        }

        [UnityTest]
        public IEnumerator WeakAndStrongBoards_ProduceDifferentWaveOutcomes()
        {
            yield return LoadPrototypeScene();

            var controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);

            controller.StartLevel(9);
            yield return null;
            controller.StartWave();
            yield return null;

            var weakResult = GetResultText().text;
            Assert.That(weakResult, Does.Contain("Defeat"));

            controller.StartLevel(9);
            var board = GetBoard(controller);
            board.SetTile(new BoardPosition(0, 0), new TileData(TileType.Wood, 3));
            board.SetTile(new BoardPosition(1, 0), new TileData(TileType.Metal, 3));
            board.SetTile(new BoardPosition(2, 0), new TileData(TileType.Food, 3));
            board.SetTile(new BoardPosition(3, 0), new TileData(TileType.Energy, 3));
            yield return null;

            controller.StartWave();
            yield return null;

            var strongResult = GetResultText().text;
            Assert.That(strongResult, Does.Contain("Victory"));
            Assert.AreNotEqual(weakResult, strongResult);
        }

        [UnityTest]
        public IEnumerator DailyRewardButton_ClaimsOnceAndUpdatesHud()
        {
            yield return LoadPrototypeScene();

            var controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);
            var dailyRewardButton = FindButton("DailyRewardButton");
            Assert.NotNull(dailyRewardButton);

            Assert.IsTrue(controller.CanClaimDailyReward);
            Assert.IsFalse(controller.HasClaimedDailyReward);
            Assert.IsTrue(dailyRewardButton.gameObject.activeSelf);

            var rewardCoins = controller.DailyRewardCoins;
            var rewardParts = controller.DailyRewardParts;
            dailyRewardButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(rewardCoins, controller.Coins);
            Assert.AreEqual(rewardParts, controller.Parts);
            Assert.IsFalse(controller.CanClaimDailyReward);
            Assert.IsTrue(controller.HasClaimedDailyReward);
            Assert.IsFalse(dailyRewardButton.gameObject.activeSelf);
            Assert.That(GetResultText().text, Does.Contain("Daily reward claimed"));
            Assert.That(GetWalletText().text, Does.Contain("claimed"));

            Assert.IsFalse(controller.ClaimDailyReward());
            Assert.AreEqual(rewardCoins, controller.Coins);
            Assert.AreEqual(rewardParts, controller.Parts);
        }

        [UnityTest]
        public IEnumerator DailyQuestButton_ClaimsFirstCompletedQuest()
        {
            yield return LoadPrototypeScene();

            var controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);
            var claimQuestButton = FindButton("ClaimQuestButton");
            Assert.NotNull(claimQuestButton);
            Assert.IsFalse(claimQuestButton.gameObject.activeSelf);

            for (var i = 0; i < 10; i++)
            {
                Assert.IsTrue(controller.TryPlaceNextTile(i % controller.BoardWidth, i / controller.BoardWidth));
                yield return null;
            }

            var placeQuest = GetQuest(controller, DailyQuestModel.PlaceTilesQuestId);
            Assert.IsTrue(placeQuest.Completed);
            Assert.IsFalse(placeQuest.Claimed);
            Assert.IsTrue(controller.CanClaimQuest);
            Assert.IsTrue(claimQuestButton.gameObject.activeSelf);
            Assert.That(GetWalletText().text, Does.Contain("Place 10 Tiles 10/10 ready"));

            var startingCoins = controller.Coins;
            var startingParts = controller.Parts;
            claimQuestButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(startingCoins + placeQuest.RewardCoins, controller.Coins);
            Assert.AreEqual(startingParts + placeQuest.RewardParts, controller.Parts);
            placeQuest = GetQuest(controller, DailyQuestModel.PlaceTilesQuestId);
            Assert.IsTrue(placeQuest.Claimed);
            Assert.IsFalse(controller.CanClaimQuest);
            Assert.IsFalse(claimQuestButton.gameObject.activeSelf);
            Assert.That(GetResultText().text, Does.Contain("Quest claimed"));
            Assert.That(GetWalletText().text, Does.Contain("Place 10 Tiles 10/10 claimed"));
        }

        [UnityTest]
        public IEnumerator SaveLoad_PersistsProgressionAcrossSceneReload()
        {
            yield return LoadPrototypeScene();

            var controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);
            Assert.IsTrue(controller.ClaimDailyReward());

            for (var i = 0; i < 10; i++)
            {
                Assert.IsTrue(controller.TryPlaceNextTile(i % controller.BoardWidth, i / controller.BoardWidth));
                yield return null;
            }

            Assert.IsTrue(controller.ClaimQuest());
            Assert.IsTrue(controller.UpgradeShelter());
            SetStrongLevelOneBoard(controller);
            controller.StartWave();
            yield return null;
            Assert.IsTrue(controller.ClaimReward());
            Assert.IsTrue(controller.StartNextLevel());
            yield return null;

            var expectedCoins = controller.Coins;
            var expectedParts = controller.Parts;
            Assert.AreEqual(2, controller.HighestUnlockedLevel);
            Assert.AreEqual(2, controller.SelectedLevel);
            Assert.AreEqual(2, controller.ShelterUpgradeLevel);
            Assert.IsTrue(controller.HasClaimedDailyReward);
            Assert.IsTrue(GetQuest(controller, DailyQuestModel.PlaceTilesQuestId).Claimed);
            Assert.IsTrue(new LocalJsonSaveService(_tempSaveDirectory).HasSave());

            yield return LoadPrototypeScene();

            controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);
            Assert.AreEqual(expectedCoins, controller.Coins);
            Assert.AreEqual(expectedParts, controller.Parts);
            Assert.AreEqual(2, controller.HighestUnlockedLevel);
            Assert.AreEqual(2, controller.SelectedLevel);
            Assert.AreEqual(2, controller.CurrentLevelId);
            Assert.AreEqual(2, controller.ShelterUpgradeLevel);
            Assert.IsTrue(controller.HasClaimedDailyReward);
            Assert.IsFalse(controller.CanClaimDailyReward);

            var placeQuest = GetQuest(controller, DailyQuestModel.PlaceTilesQuestId);
            Assert.AreEqual(10, placeQuest.Progress);
            Assert.IsTrue(placeQuest.Completed);
            Assert.IsTrue(placeQuest.Claimed);

            var completeQuest = GetQuest(controller, DailyQuestModel.CompleteLevelQuestId);
            Assert.AreEqual(1, completeQuest.Progress);
            Assert.IsTrue(completeQuest.Completed);

            var claimRewardQuest = GetQuest(controller, DailyQuestModel.ClaimRewardQuestId);
            Assert.AreEqual(1, claimRewardQuest.Progress);
            Assert.IsTrue(claimRewardQuest.Completed);
        }

        [UnityTest]
        public IEnumerator ResetSaveButton_ReturnsToNewPlayerState()
        {
            yield return LoadPrototypeScene();

            var controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);
            Assert.IsTrue(controller.ClaimDailyReward());
            Assert.Greater(controller.Coins, 0);
            Assert.IsTrue(new LocalJsonSaveService(_tempSaveDirectory).HasSave());

            var resetSaveButton = FindButton("ResetSaveButton");
            Assert.NotNull(resetSaveButton);
            Assert.IsTrue(resetSaveButton.gameObject.activeSelf);

            resetSaveButton.onClick.Invoke();
            yield return null;

            AssertNewPlayerState(controller);
            Assert.IsFalse(new LocalJsonSaveService(_tempSaveDirectory).HasSave());
            Assert.That(GetResultText().text, Does.Contain("Save reset"));

            yield return LoadPrototypeScene();

            controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);
            AssertNewPlayerState(controller);
        }

        [UnityTest]
        public IEnumerator DoubleRewardButton_DoublesPendingRewardOnceBeforeClaim()
        {
            yield return LoadPrototypeScene();

            var controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);
            controller.StartLevel(9);

            var doubleRewardButton = FindButton("DoubleRewardButton");
            Assert.NotNull(doubleRewardButton);
            Assert.IsFalse(controller.CanDoubleReward);
            Assert.IsFalse(doubleRewardButton.gameObject.activeSelf);

            SetStrongLevelTenBoard(controller);
            controller.StartWave();
            yield return null;

            Assert.IsTrue(controller.HasPendingReward);
            Assert.IsTrue(controller.CanDoubleReward);
            Assert.IsTrue(doubleRewardButton.gameObject.activeSelf);
            Assert.AreEqual(250, controller.PendingRewardCoins);
            Assert.AreEqual(5, controller.PendingRewardParts);

            doubleRewardButton.onClick.Invoke();
            yield return null;

            Assert.IsFalse(controller.CanDoubleReward);
            Assert.IsFalse(doubleRewardButton.gameObject.activeSelf);
            Assert.AreEqual(500, controller.PendingRewardCoins);
            Assert.AreEqual(10, controller.PendingRewardParts);
            Assert.That(GetResultText().text, Does.Contain("Reward doubled"));
            Assert.IsFalse(controller.DoubleReward());
            Assert.AreEqual(500, controller.PendingRewardCoins);
            Assert.AreEqual(10, controller.PendingRewardParts);

            Assert.IsTrue(controller.ClaimReward());
            yield return null;

            Assert.AreEqual(500, controller.Coins);
            Assert.AreEqual(10, controller.Parts);
            Assert.IsFalse(controller.HasPendingReward);
            Assert.IsFalse(controller.CanDoubleReward);
        }

        [UnityTest]
        public IEnumerator ReviveButton_HidesAndIgnoresStaleClicksAfterSuccessfulRevive()
        {
            yield return LoadPrototypeScene();

            var controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);
            controller.StartLevel(9);

            var reviveButton = FindButton("ReviveButton");
            var startWaveButton = FindButton("StartWaveButton");
            var retryButton = FindButton("RetryButton");
            Assert.NotNull(reviveButton);
            Assert.NotNull(startWaveButton);
            Assert.NotNull(retryButton);
            Assert.IsFalse(controller.CanRevive);
            Assert.IsFalse(reviveButton.gameObject.activeSelf);

            controller.StartWave();
            yield return null;

            Assert.IsTrue(controller.IsLevelEnded);
            Assert.IsTrue(controller.CanRevive);
            Assert.IsTrue(reviveButton.gameObject.activeSelf);
            Assert.IsFalse(startWaveButton.gameObject.activeSelf);

            reviveButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(10, controller.CurrentLevelId);
            Assert.AreEqual(10, controller.SelectedLevel);
            Assert.IsFalse(controller.IsLevelEnded);
            Assert.IsFalse(controller.CanRevive);
            Assert.IsFalse(reviveButton.gameObject.activeSelf);
            Assert.IsFalse(reviveButton.interactable);
            Assert.IsFalse(retryButton.gameObject.activeSelf);
            Assert.IsTrue(startWaveButton.gameObject.activeSelf);
            Assert.IsTrue(controller.GetTileAt(0, 0).IsEmpty);
            Assert.That(GetResultText().text, Does.Contain("Revive used"));
            Assert.IsFalse(controller.Revive());
            Assert.AreEqual(10, controller.CurrentLevelId);
            Assert.AreEqual(SceneName, SceneManager.GetActiveScene().name);
            Assert.IsFalse(controller.IsLevelEnded);
            Assert.IsFalse(controller.CanRevive);
            Assert.IsFalse(reviveButton.gameObject.activeSelf);
            Assert.IsTrue(startWaveButton.gameObject.activeSelf);

            var resultBeforeStaleClick = GetResultText().text;
            reviveButton.onClick.Invoke();
            yield return null;

            Assert.NotNull(Object.FindObjectOfType<PrototypeBoardView>());
            Assert.NotNull(Object.FindObjectOfType<Canvas>());
            Assert.AreEqual(10, controller.CurrentLevelId);
            Assert.IsFalse(controller.IsLevelEnded);
            Assert.IsFalse(controller.CanRevive);
            Assert.IsFalse(reviveButton.gameObject.activeSelf);
            Assert.IsFalse(reviveButton.interactable);
            Assert.IsTrue(startWaveButton.gameObject.activeSelf);
            Assert.AreEqual(resultBeforeStaleClick, GetResultText().text);
        }

        [UnityTest]
        public IEnumerator Revive_RemainsUnavailableIfRevivedAttemptFailsAgain()
        {
            yield return LoadPrototypeScene();

            var controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);
            controller.StartLevel(9);

            var reviveButton = FindButton("ReviveButton");
            var retryButton = FindButton("RetryButton");
            Assert.NotNull(reviveButton);
            Assert.NotNull(retryButton);

            controller.StartWave();
            yield return null;

            Assert.IsTrue(controller.CanRevive);
            reviveButton.onClick.Invoke();
            yield return null;

            Assert.IsFalse(controller.CanRevive);
            Assert.IsFalse(reviveButton.gameObject.activeSelf);

            controller.StartWave();
            yield return null;

            Assert.IsTrue(controller.IsLevelEnded);
            Assert.IsTrue(controller.CanRetryLevel);
            Assert.IsTrue(retryButton.gameObject.activeSelf);
            Assert.IsFalse(controller.CanRevive);
            Assert.IsFalse(reviveButton.gameObject.activeSelf);
            Assert.IsFalse(controller.Revive());
        }

        [UnityTest]
        public IEnumerator RewardClaim_UnlocksLevelTwoAndNextLevelButtonStartsIt()
        {
            yield return LoadPrototypeScene();

            var controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);
            controller.StartLevel(0);
            Assert.IsFalse(controller.TryStartLevel(2));
            Assert.AreEqual(1, controller.CurrentLevelId);
            SetStrongLevelOneBoard(controller);

            var claimButton = FindButton("ClaimRewardButton");
            var nextLevelButton = FindButton("NextLevelButton");
            var startWaveButton = FindButton("StartWaveButton");
            Assert.NotNull(claimButton);
            Assert.NotNull(nextLevelButton);
            Assert.NotNull(startWaveButton);
            Assert.IsFalse(claimButton.gameObject.activeSelf);
            Assert.IsFalse(nextLevelButton.gameObject.activeSelf);
            Assert.IsTrue(startWaveButton.gameObject.activeSelf);

            controller.StartWave();
            yield return null;

            Assert.IsTrue(controller.HasPendingReward);
            Assert.AreEqual(0, controller.Coins);
            Assert.AreEqual(1, controller.HighestUnlockedLevel);
            Assert.IsTrue(claimButton.gameObject.activeSelf);
            Assert.IsFalse(startWaveButton.gameObject.activeSelf);

            claimButton.onClick.Invoke();
            yield return null;

            Assert.IsFalse(controller.HasPendingReward);
            Assert.AreEqual(50, controller.Coins);
            Assert.AreEqual(0, controller.Parts);
            Assert.AreEqual(2, controller.HighestUnlockedLevel);
            Assert.IsFalse(controller.ClaimReward());
            Assert.AreEqual(50, controller.Coins);
            Assert.IsTrue(nextLevelButton.gameObject.activeSelf);

            nextLevelButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(2, controller.CurrentLevelId);
            Assert.AreEqual(2, controller.SelectedLevel);
            Assert.IsFalse(controller.IsLevelEnded);
            Assert.IsFalse(nextLevelButton.gameObject.activeSelf);
            Assert.IsTrue(startWaveButton.gameObject.activeSelf);
        }

        [UnityTest]
        public IEnumerator Retry_RestartsSelectedLevelAfterDefeat()
        {
            yield return LoadPrototypeScene();

            var controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);
            controller.StartLevel(9);
            Assert.AreEqual(10, controller.CurrentLevelId);

            controller.StartWave();
            yield return null;

            var retryButton = FindButton("RetryButton");
            Assert.NotNull(retryButton);
            Assert.IsTrue(controller.IsLevelEnded);
            Assert.IsTrue(controller.CanRetryLevel);
            Assert.IsTrue(retryButton.gameObject.activeSelf);

            retryButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(10, controller.CurrentLevelId);
            Assert.AreEqual(10, controller.SelectedLevel);
            Assert.IsFalse(controller.IsLevelEnded);
            Assert.IsFalse(controller.CanRetryLevel);
            Assert.IsFalse(retryButton.gameObject.activeSelf);
            Assert.IsTrue(controller.GetTileAt(0, 0).IsEmpty);
        }

        [UnityTest]
        public IEnumerator UpgradeShelterButton_SpendsCoinsAndImprovesNextLevelMaxHp()
        {
            yield return LoadPrototypeScene();

            var controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);
            controller.StartLevel(0);

            var upgradeButton = FindButton("UpgradeShelterButton");
            Assert.NotNull(upgradeButton);
            Assert.IsTrue(upgradeButton.gameObject.activeSelf);
            Assert.AreEqual(1, controller.ShelterUpgradeLevel);
            Assert.AreEqual(100, controller.ShelterUpgradeCost);
            Assert.IsFalse(controller.CanAffordShelterUpgrade);

            upgradeButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(1, controller.ShelterUpgradeLevel);
            Assert.AreEqual(0, controller.Coins);
            Assert.That(GetResultText().text, Does.Contain("Upgrade blocked"));

            yield return WinCurrentLevelAndClaim(controller);
            Assert.IsTrue(controller.StartNextLevel());
            yield return null;

            yield return WinCurrentLevelAndClaim(controller);
            Assert.AreEqual(120, controller.Coins);
            Assert.IsTrue(controller.CanAffordShelterUpgrade);

            var previousCost = controller.ShelterUpgradeCost;
            upgradeButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(2, controller.ShelterUpgradeLevel);
            Assert.AreEqual(20, controller.Coins);
            Assert.Greater(controller.ShelterUpgradeCost, previousCost);
            Assert.That(GetResultText().text, Does.Contain("Shelter upgraded"));
            Assert.That(GetWalletText().text, Does.Contain("Shelter Lv 2"));

            Assert.IsTrue(controller.StartNextLevel());
            yield return null;

            Assert.AreEqual(3, controller.CurrentLevelId);
            Assert.Greater(controller.CurrentShelterMaxHp, 100);
            Assert.That(GetShelterHpText().text, Does.Contain("125/125"));
        }

        private static IEnumerator LoadPrototypeScene()
        {
            var loadOperation = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
            while (!loadOperation.isDone)
                yield return null;

            yield return null;
        }

        private static Text GetResultText()
        {
            var resultText = GameObject.Find("ResultText")?.GetComponent<Text>();
            Assert.NotNull(resultText);
            return resultText;
        }

        private static Text GetWalletText()
        {
            var walletText = GameObject.Find("WalletText")?.GetComponent<Text>();
            Assert.NotNull(walletText);
            return walletText;
        }

        private static Text GetShelterHpText()
        {
            var shelterHpText = GameObject.Find("ShelterHpText")?.GetComponent<Text>();
            Assert.NotNull(shelterHpText);
            return shelterHpText;
        }

        private static Button FindButton(string name)
        {
            foreach (var button in Object.FindObjectsOfType<Button>(true))
            {
                if (button.name == name)
                    return button;
            }

            return null;
        }

        private static BoardModel GetBoard(PrototypeGameController controller)
        {
            var field = typeof(PrototypeGameController).GetField("_board", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(field);
            return (BoardModel)field.GetValue(controller);
        }

        private static DailyQuestState GetQuest(PrototypeGameController controller, string questId)
        {
            foreach (var quest in controller.DailyQuests)
            {
                if (quest.QuestId == questId)
                    return quest;
            }

            Assert.Fail($"Quest not found: {questId}");
            return default;
        }

        private static void AssertNewPlayerState(PrototypeGameController controller)
        {
            Assert.AreEqual(1, controller.CurrentLevelId);
            Assert.AreEqual(1, controller.SelectedLevel);
            Assert.AreEqual(1, controller.HighestUnlockedLevel);
            Assert.AreEqual(0, controller.Coins);
            Assert.AreEqual(0, controller.Parts);
            Assert.AreEqual(1, controller.ShelterUpgradeLevel);
            Assert.IsTrue(controller.CanClaimDailyReward);
            Assert.IsFalse(controller.HasClaimedDailyReward);
            Assert.AreEqual(0, GetQuest(controller, DailyQuestModel.PlaceTilesQuestId).Progress);
            Assert.AreEqual(0, GetQuest(controller, DailyQuestModel.CompleteLevelQuestId).Progress);
            Assert.AreEqual(0, GetQuest(controller, DailyQuestModel.ClaimRewardQuestId).Progress);
            Assert.IsTrue(controller.GetTileAt(0, 0).IsEmpty);
        }

        private static void SetStrongLevelOneBoard(PrototypeGameController controller)
        {
            var board = GetBoard(controller);
            board.SetTile(new BoardPosition(0, 0), new TileData(TileType.Wood, 3));
        }

        private static void SetStrongLevelTenBoard(PrototypeGameController controller)
        {
            var board = GetBoard(controller);
            board.SetTile(new BoardPosition(0, 0), new TileData(TileType.Wood, 3));
            board.SetTile(new BoardPosition(1, 0), new TileData(TileType.Metal, 3));
            board.SetTile(new BoardPosition(2, 0), new TileData(TileType.Food, 3));
            board.SetTile(new BoardPosition(3, 0), new TileData(TileType.Energy, 3));
        }

        private static IEnumerator WinCurrentLevelAndClaim(PrototypeGameController controller)
        {
            SetStrongLevelOneBoard(controller);
            controller.StartWave();
            yield return null;
            Assert.IsTrue(controller.CanClaimReward);
            Assert.IsTrue(controller.ClaimReward());
            yield return null;
        }
    }
}
