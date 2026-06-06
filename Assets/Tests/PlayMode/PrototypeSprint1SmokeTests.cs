using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MergeShelter.Board;
using MergeShelter.Combat;
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
        private static readonly string[] HudTextNames =
        {
            "LevelText",
            PrototypeHudView.ObjectiveTextName,
            "TutorialText",
            PrototypeHudView.ShelterSectionLabelName,
            "ShelterHpText",
            PrototypeHudView.ShelterUpgradeTextName,
            PrototypeHudView.BoardSectionLabelName,
            "NextTileText",
            PrototypeHudView.WaveRosterTextName,
            PrototypeHudView.ActionsSectionLabelName,
            PrototypeHudView.RewardsSectionLabelName,
            "WalletText",
            PrototypeHudView.RewardTextName,
            PrototypeHudView.QuestsSectionLabelName,
            PrototypeHudView.QuestTextName,
            "ResultText"
        };

        private static readonly string[] ActionButtonNames =
        {
            "StartWaveButton",
            "ClaimRewardButton",
            "NextLevelButton",
            "RetryButton",
            "UpgradeShelterButton",
            "DailyRewardButton",
            "ClaimQuestButton",
            "DoubleRewardButton",
            "ReviveButton",
            "PreviousLevelButton",
            "ReplayLevelButton",
            "NextUnlockedLevelButton",
            "ResetSaveButton"
        };

        private static readonly string[] HudPanelNames =
        {
            PrototypeHudView.HeaderPanelName,
            PrototypeHudView.ShelterPanelName,
            PrototypeHudView.RewardsPanelName,
            PrototypeHudView.QuestsPanelName,
            PrototypeHudView.BoardInfoPanelName
        };

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

            var objectiveText = GetHudText(PrototypeHudView.ObjectiveTextName);
            Assert.That(objectiveText.text, Does.Contain("Goal:"));
            Assert.That(objectiveText.text, Does.Contain("Survive"));

            var waveRosterText = GetHudText(PrototypeHudView.WaveRosterTextName);
            Assert.That(waveRosterText.text, Does.StartWith("Wave:"));
            Assert.That(waveRosterText.text, Does.Contain("Walker"));

            var firstCell = GameObject.Find("Cell_0_0")?.GetComponent<Button>();
            Assert.NotNull(firstCell);
            AssertColorApproximately(PrototypeVisualKit.EmptyCell, GetCellImage(0, 0).color);
            AssertBoardPanelStyle();
            AssertCellUsesVisualBorder(0, 0);
            AssertColorApproximately(PrototypeVisualKit.GetActionButtonColor("StartWaveButton", true), GetButtonImage("StartWaveButton").color);

            firstCell.onClick.Invoke();
            yield return null;

            Assert.IsFalse(controller.GetTileAt(0, 0).IsEmpty);
            yield return new WaitForSeconds(0.32f);
            AssertColorApproximately(
                PrototypeVisualKit.GetTileFillColor(new TileData(TileType.Wood, 1)),
                GetCellImage(0, 0).color);
            Assert.That(GameObject.Find("Cell_0_0/Label")?.GetComponent<Text>()?.text, Does.Contain("[W]"));
            AssertFilledCellUsesHighlightBorder(0, 0);

            var startWaveButton = GameObject.Find("StartWaveButton")?.GetComponent<Button>();
            Assert.NotNull(startWaveButton);

            startWaveButton.onClick.Invoke();
            yield return null;

            Assert.IsFalse(string.IsNullOrWhiteSpace(GetResultText().text));
            Assert.That(GetHudText(PrototypeHudView.WaveRosterTextName).text, Is.Empty,
                "Wave roster should clear after wave resolves.");
        }

        [UnityTest]
        public IEnumerator GameplayFeedback_ShowsDistinctBoardActionStates()
        {
            yield return LoadPrototypeScene();

            var controller = Object.FindObjectOfType<PrototypeGameController>();
            var boardView = Object.FindObjectOfType<PrototypeBoardView>();
            var hudView = Object.FindObjectOfType<PrototypeHudView>();
            Assert.NotNull(controller);
            Assert.NotNull(boardView);
            Assert.NotNull(hudView);

            var firstCell = FindCellButton(0, 0);
            var firstCellImage = GetCellImage(0, 0);
            var emptyColor = firstCellImage.color;

            firstCell.onClick.Invoke();
            yield return null;

            AssertFeedback(controller, PrototypeFeedbackKind.TilePlaced, "TILE:");
            Assert.AreEqual(PrototypeFeedbackKind.TilePlaced, hudView.CurrentFeedbackKind);
            Assert.AreEqual(PrototypeFeedbackKind.TilePlaced, boardView.LastCellFeedbackKind);
            Assert.IsTrue(boardView.HasActiveCellFeedback);
            Assert.AreNotEqual(emptyColor, firstCellImage.color);
            Assert.That(GetCellScale(0, 0), Is.EqualTo(PrototypeVisualKit.GetCellFeedbackScale(PrototypeFeedbackKind.TilePlaced)).Within(0.01f));

            firstCell.onClick.Invoke();
            yield return null;

            AssertFeedback(controller, PrototypeFeedbackKind.InvalidPlacement, "BLOCKED:");
            Assert.That(GetResultText().text, Does.Contain("merge"));
            Assert.AreEqual(PrototypeFeedbackKind.InvalidPlacement, boardView.LastCellFeedbackKind);
            Assert.That(GetCellScale(0, 0), Is.EqualTo(PrototypeVisualKit.GetCellFeedbackScale(PrototypeFeedbackKind.InvalidPlacement)).Within(0.01f));

            FindCellButton(1, 0).onClick.Invoke();
            yield return null;
            AssertFeedback(controller, PrototypeFeedbackKind.TilePlaced, "TILE:");

            FindCellButton(2, 0).onClick.Invoke();
            yield return null;

            AssertFeedback(controller, PrototypeFeedbackKind.MergeSuccess, "MERGE:");
            Assert.AreEqual(PrototypeFeedbackKind.MergeSuccess, boardView.LastCellFeedbackKind);
            Assert.That(GetCellScale(2, 0), Is.EqualTo(PrototypeVisualKit.GetCellFeedbackScale(PrototypeFeedbackKind.MergeSuccess)).Within(0.01f));
            Assert.IsTrue(controller.HasShownFeedback(PrototypeFeedbackKind.TilePlaced));
            Assert.IsTrue(controller.HasShownFeedback(PrototypeFeedbackKind.InvalidPlacement));
            Assert.IsTrue(controller.HasShownFeedback(PrototypeFeedbackKind.MergeSuccess));

            yield return new WaitForSeconds(Mathf.Max(boardView.CellEffectDurationSeconds, hudView.ResultEffectDurationSeconds) + 0.05f);

            Assert.IsFalse(boardView.HasActiveCellFeedback);
            Assert.IsFalse(hudView.HasActiveResultEffect);
            Assert.That(GetCellScale(2, 0), Is.EqualTo(1f).Within(0.01f));
            AssertResultPanelNormal(hudView.CurrentFeedbackKind);
        }

        [UnityTest]
        public IEnumerator PhoneHudLayout_KeepsTextBoardAndActionsSeparatedAcrossProgressionStates()
        {
            Screen.SetResolution(720, 1280, false);
            yield return null;

            yield return LoadPrototypeScene();
            yield return null;

            var controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);

            AssertRequiredPhoneUiExists();
            AssertVisibleActionButton("StartWaveButton");
            AssertPrimaryActionButton("StartWaveButton");
            AssertVisibleActionButton("UpgradeShelterButton");
            AssertVisibleActionButton("ResetSaveButton");
            AssertVisibleActionButton("DailyRewardButton");
            AssertPhoneSafeLayout();

            FindButton("DailyRewardButton").onClick.Invoke();
            yield return null;
            AssertPhoneSafeLayout();
            Assert.That(GetRewardText().text, Does.Contain("claimed"));
            AssertQuestTextIsCompact();

            for (var i = 0; i < 10; i++)
            {
                Assert.IsTrue(controller.TryPlaceNextTile(i % controller.BoardWidth, i / controller.BoardWidth));
                yield return null;

                if (i == 0)
                    Assert.That(GetResultText().text, Does.Contain("Quest: Place 10 Tiles 1/10"));
            }

            AssertVisibleActionButton("ClaimQuestButton");
            AssertPhoneSafeLayout();

            FindButton("ClaimQuestButton").onClick.Invoke();
            yield return null;
            AssertPhoneSafeLayout();

            controller.StartLevel(0);
            SetStrongLevelOneBoard(controller);
            controller.StartWave();
            yield return null;

            AssertVisibleActionButton("ClaimRewardButton");
            AssertPrimaryActionButton("ClaimRewardButton");
            AssertPhoneSafeLayout();

            FindButton("ClaimRewardButton").onClick.Invoke();
            yield return null;
            AssertPrimaryActionButton("NextLevelButton");
            AssertPhoneSafeLayout();

            AssertVisibleActionButton("UpgradeShelterButton");
            FindButton("UpgradeShelterButton").onClick.Invoke();
            yield return null;
            Assert.That(GetResultText().text, Does.Contain("Shelter upgraded"));
            AssertPhoneSafeLayout();

            FindButton("ResetSaveButton").onClick.Invoke();
            yield return null;
            AssertNewPlayerState(controller);
            AssertPhoneSafeLayout();

            controller.StartLevel(9);
            SetStrongLevelTenBoard(controller);
            controller.StartWave();
            yield return null;

            AssertVisibleActionButton("ClaimRewardButton");
            AssertVisibleActionButton("DoubleRewardButton");
            AssertPhoneSafeLayout();

            FindButton("DoubleRewardButton").onClick.Invoke();
            yield return null;
            AssertPhoneSafeLayout();

            controller.StartLevel(9);
            controller.StartWave();
            yield return null;

            AssertVisibleActionButton("RetryButton");
            AssertVisibleActionButton("ReviveButton");
            AssertPrimaryActionButton("RetryButton");
            AssertPrimaryActionButton("ReviveButton");
            AssertPhoneSafeLayout();

            FindButton("ReviveButton").onClick.Invoke();
            yield return null;
            AssertPrimaryActionButton("StartWaveButton");
            AssertPhoneSafeLayout();

            FindButton("ResetSaveButton").onClick.Invoke();
            yield return null;
            AssertNewPlayerState(controller);
            AssertPhoneSafeLayout();
        }

        [UnityTest]
        public IEnumerator FirstRunTutorial_AdvancesThroughLevelOneActionsAndReset()
        {
            yield return LoadPrototypeScene();

            var controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);
            Assert.AreEqual(PrototypeTutorialStep.PlaceFirstTile, controller.TutorialStep);
            Assert.That(GetTutorialText().text, Does.Contain("tap an empty board cell"));

            Assert.IsTrue(controller.TryPlaceNextTile(0, 0));
            yield return null;
            Assert.AreEqual(PrototypeTutorialStep.PlaceMoreTiles, controller.TutorialStep);
            Assert.AreEqual(1, controller.TutorialTilesPlaced);
            Assert.That(GetTutorialText().text, Does.Contain("place two more"));

            Assert.IsTrue(controller.TryPlaceNextTile(1, 0));
            yield return null;
            Assert.AreEqual(PrototypeTutorialStep.MergeIntent, controller.TutorialStep);
            Assert.AreEqual(2, controller.TutorialTilesPlaced);
            Assert.That(GetTutorialText().text, Does.Contain("one more"));

            Assert.IsTrue(controller.TryPlaceNextTile(2, 0));
            yield return null;
            Assert.AreEqual(PrototypeTutorialStep.StartWave, controller.TutorialStep);
            Assert.AreEqual(3, controller.TutorialTilesPlaced);
            Assert.That(GetTutorialText().text, Does.Contain("Start Wave"));

            controller.StartWave();
            yield return null;
            Assert.AreEqual(PrototypeTutorialStep.ClaimReward, controller.TutorialStep);
            Assert.IsTrue(controller.CanClaimReward);
            Assert.That(GetTutorialText().text, Does.Contain("Claim Reward"));

            Assert.IsTrue(controller.ClaimReward());
            yield return null;
            Assert.AreEqual(PrototypeTutorialStep.Continue, controller.TutorialStep);
            Assert.IsTrue(controller.CanStartNextLevel);
            Assert.That(GetTutorialText().text, Does.Contain("Next Level"));

            Assert.IsTrue(controller.StartNextLevel());
            yield return null;
            Assert.AreEqual(PrototypeTutorialStep.Complete, controller.TutorialStep);
            Assert.IsTrue(controller.IsTutorialComplete);
            Assert.AreEqual(2, controller.CurrentLevelId);

            controller.ResetSave();
            yield return null;
            AssertNewPlayerState(controller);
            Assert.AreEqual(PrototypeTutorialStep.PlaceFirstTile, controller.TutorialStep);
            Assert.AreEqual(0, controller.TutorialTilesPlaced);
            Assert.That(GetTutorialText().text, Does.Contain("tap an empty board cell"));
        }

        [UnityTest]
        public IEnumerator FirstRunTutorial_SaveLoadPreservesProgress()
        {
            yield return LoadPrototypeScene();

            var controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);
            Assert.IsTrue(controller.TryPlaceNextTile(0, 0));
            yield return null;
            Assert.AreEqual(PrototypeTutorialStep.PlaceMoreTiles, controller.TutorialStep);
            Assert.AreEqual(1, controller.TutorialTilesPlaced);
            Assert.IsTrue(new LocalJsonSaveService(_tempSaveDirectory).HasSave());

            yield return LoadPrototypeScene();

            controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);
            Assert.AreEqual(PrototypeTutorialStep.PlaceMoreTiles, controller.TutorialStep);
            Assert.AreEqual(1, controller.TutorialTilesPlaced);
            Assert.That(GetTutorialText().text, Does.Contain("place two more"));
        }

        [UnityTest]
        public IEnumerator FirstRunTutorial_DoesNotBlockNormalGameplay()
        {
            yield return LoadPrototypeScene();

            var controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);
            Assert.AreEqual(PrototypeTutorialStep.PlaceFirstTile, controller.TutorialStep);

            controller.StartLevel(9);
            yield return null;
            controller.StartWave();
            yield return null;

            Assert.AreEqual(10, controller.CurrentLevelId);
            Assert.IsTrue(controller.IsLevelEnded);
            Assert.IsTrue(controller.CanRetryLevel);
            AssertVisibleActionButton("RetryButton");
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
            Assert.That(weakResult, Does.Not.Contain("weak_wall"));
            Assert.That(weakResult, Does.Not.Contain("low_attack"));
            Assert.That(weakResult, Does.Not.Contain("overwhelmed"));
            AssertDefeatCopyHasActionableHint(weakResult);
            AssertFeedback(controller, PrototypeFeedbackKind.WaveDefeat, "DEFEAT:");
            Assert.IsTrue(controller.HasShownFeedback(PrototypeFeedbackKind.WaveStart));

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
            Assert.That(strongResult, Does.Contain("Objective complete"));
            AssertVictoryCopyDoesNotContainDefeatLanguage(strongResult);
            AssertFeedback(controller, PrototypeFeedbackKind.WaveVictory, "WIN:");
            Assert.IsTrue(controller.HasShownFeedback(PrototypeFeedbackKind.WaveStart));
            Assert.AreNotEqual(weakResult, strongResult);
        }

        [UnityTest]
        public IEnumerator SurvivableDamageVictory_UsesVictoryCopyOnly()
        {
            yield return LoadPrototypeScene();

            var controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);

            controller.StartLevel(1);
            yield return null;
            Assert.AreEqual(2, controller.CurrentLevelId);

            controller.StartWave();
            yield return null;

            Assert.IsTrue(controller.IsLevelEnded);
            Assert.IsTrue(controller.CanClaimReward);
            AssertFeedback(controller, PrototypeFeedbackKind.WaveVictory, "WIN:");

            var result = GetResultText().text;
            AssertVictoryCopyDoesNotContainDefeatLanguage(result);
            Assert.That(result, Does.Contain("Objective complete"));
            Assert.That(result, Does.Contain("Reward pending"));

            Assert.IsTrue(controller.ClaimReward());
            yield return null;
            Assert.AreEqual(3, controller.HighestUnlockedLevel);
            Assert.IsTrue(controller.CanStartNextLevel);
            AssertFeedback(controller, PrototypeFeedbackKind.RewardClaim, "REWARD:");
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
            AssertFeedback(controller, PrototypeFeedbackKind.DailyRewardClaim, "DAILY:");
            Assert.That(GetRewardText().text, Does.Contain("claimed"));

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
            Assert.That(GetQuestText().text, Does.Contain("Place 10 Tiles: 0/10"));
            Assert.That(GetQuestText().text, Does.Contain("Complete 1 Level: 0/1"));
            Assert.That(GetQuestText().text, Does.Contain("Claim 1 Reward: 0/1"));
            Assert.That(GetQuestText().text, Does.Not.Contain("+40c"), "Quest rewards should stay hidden until a quest is ready to reduce visual density.");
            AssertQuestTextIsCompact();

            Assert.IsFalse(controller.ClaimQuest());
            Assert.That(GetResultText().text, Does.Contain("No quest ready"));
            Assert.That(GetResultText().text, Does.Contain("claimed quests pay once"));
            AssertFeedback(controller, PrototypeFeedbackKind.Blocked, "BLOCKED:");

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
            Assert.That(GetQuestText().text, Does.Contain("Place 10 Tiles: 10/10 READY"));
            Assert.That(GetQuestText().text, Does.Contain("+40c"));
            Assert.That(GetResultText().text, Does.Contain("Quest ready: Place 10 Tiles"));
            Assert.That(GetResultText().text, Does.Contain("+40c"));

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
            Assert.That(GetResultText().text, Does.Contain("+40 coins"));
            Assert.That(GetResultText().text, Does.Contain("+0 parts"));
            AssertFeedback(controller, PrototypeFeedbackKind.QuestClaim, "QUEST:");
            Assert.That(GetQuestText().text, Does.Contain("Place 10 Tiles: 10/10 claimed"));
            Assert.That(GetQuestText().text, Does.Not.Contain("Place 10 Tiles: 10/10 READY"));

            Assert.IsFalse(controller.ClaimQuest());
            Assert.AreEqual(startingCoins + placeQuest.RewardCoins, controller.Coins);
            Assert.AreEqual(startingParts + placeQuest.RewardParts, controller.Parts);
            Assert.That(GetResultText().text, Does.Contain("No quest ready"));
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
            Assert.That(GetShelterUpgradeText().text, Does.Contain("Lv 2"));
            Assert.That(GetRewardText().text, Does.Contain("claimed"));
            Assert.That(GetQuestText().text, Does.Contain("Place 10 Tiles: 10/10 claimed"));
            Assert.That(GetQuestText().text, Does.Contain("Complete 1 Level: 1/1 READY"));
            Assert.That(GetQuestText().text, Does.Contain("Claim 1 Reward: 1/1 READY"));

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
            Assert.That(GetQuestText().text, Does.Contain("Place 10 Tiles: 0/10"));
            Assert.That(GetQuestText().text, Does.Contain("Complete 1 Level: 0/1"));
            Assert.That(GetQuestText().text, Does.Contain("Claim 1 Reward: 0/1"));
            AssertFeedback(controller, PrototypeFeedbackKind.ResetSave, "RESET:");

            yield return LoadPrototypeScene();

            controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);
            AssertNewPlayerState(controller);
            Assert.That(GetQuestText().text, Does.Contain("Place 10 Tiles: 0/10"));
            Assert.That(GetQuestText().text, Does.Contain("Complete 1 Level: 0/1"));
            Assert.That(GetQuestText().text, Does.Contain("Claim 1 Reward: 0/1"));
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
            AssertFeedback(controller, PrototypeFeedbackKind.RewardDouble, "DOUBLE:");
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
            Assert.That(GetResultText().text, Does.Contain("Goal:"));
            AssertFeedback(controller, PrototypeFeedbackKind.Revive, "REVIVE:");
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
            AssertFeedback(controller, PrototypeFeedbackKind.RewardClaim, "REWARD:");
            Assert.IsFalse(controller.ClaimReward());
            Assert.AreEqual(50, controller.Coins);
            Assert.IsTrue(nextLevelButton.gameObject.activeSelf);
            AssertFeedback(controller, PrototypeFeedbackKind.Blocked, "BLOCKED:");

            nextLevelButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(2, controller.CurrentLevelId);
            Assert.AreEqual(2, controller.SelectedLevel);
            Assert.IsFalse(controller.IsLevelEnded);
            Assert.IsFalse(nextLevelButton.gameObject.activeSelf);
            Assert.IsTrue(startWaveButton.gameObject.activeSelf);
            AssertFeedback(controller, PrototypeFeedbackKind.NextLevel, "NEXT:");
        }

        [UnityTest]
        public IEnumerator LevelNavigationButtons_SelectOnlyUnlockedLevelsAndPersistSelection()
        {
            yield return LoadPrototypeScene();

            var controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);

            var previousButton = FindButton("PreviousLevelButton");
            var replayButton = FindButton("ReplayLevelButton");
            var nextUnlockedButton = FindButton("NextUnlockedLevelButton");
            Assert.NotNull(previousButton);
            Assert.NotNull(replayButton);
            Assert.NotNull(nextUnlockedButton);

            Assert.IsFalse(controller.CanSelectPreviousUnlockedLevel);
            Assert.IsTrue(controller.CanReplaySelectedLevel);
            Assert.IsFalse(controller.CanSelectNextUnlockedLevel);
            Assert.IsFalse(previousButton.gameObject.activeSelf);
            Assert.IsTrue(replayButton.gameObject.activeSelf);
            Assert.IsFalse(nextUnlockedButton.gameObject.activeSelf);

            Assert.IsFalse(controller.TryStartLevel(2));
            Assert.AreEqual(1, controller.SelectedLevel);
            Assert.That(GetResultText().text, Does.Contain("locked"));
            AssertFeedback(controller, PrototypeFeedbackKind.Blocked, "BLOCKED:");

            SetStrongLevelOneBoard(controller);
            controller.StartWave();
            yield return null;

            Assert.IsTrue(controller.HasPendingReward);
            Assert.IsFalse(controller.TryStartLevel(1));
            Assert.AreEqual(1, controller.SelectedLevel);
            Assert.That(GetResultText().text, Does.Contain("pending reward"));
            Assert.IsFalse(previousButton.gameObject.activeSelf);
            Assert.IsFalse(replayButton.gameObject.activeSelf);
            Assert.IsFalse(nextUnlockedButton.gameObject.activeSelf);

            Assert.IsTrue(controller.ClaimReward());
            yield return null;

            Assert.AreEqual(2, controller.HighestUnlockedLevel);
            Assert.IsFalse(controller.HasPendingReward);
            Assert.IsTrue(controller.CanStartNextLevel);
            Assert.IsFalse(previousButton.gameObject.activeSelf);
            Assert.IsFalse(replayButton.gameObject.activeSelf);
            Assert.IsFalse(nextUnlockedButton.gameObject.activeSelf);

            Assert.IsTrue(controller.StartNextLevel());
            yield return null;

            Assert.AreEqual(2, controller.CurrentLevelId);
            Assert.AreEqual(2, controller.SelectedLevel);
            Assert.IsTrue(previousButton.gameObject.activeSelf);
            Assert.IsTrue(replayButton.gameObject.activeSelf);
            Assert.IsFalse(nextUnlockedButton.gameObject.activeSelf);

            Assert.IsTrue(controller.TryPlaceNextTile(0, 0));
            yield return null;
            Assert.IsFalse(controller.GetTileAt(0, 0).IsEmpty);

            previousButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(1, controller.CurrentLevelId);
            Assert.AreEqual(1, controller.SelectedLevel);
            Assert.IsTrue(controller.GetTileAt(0, 0).IsEmpty);
            Assert.That(GetResultText().text, Does.Contain("Previous"));
            AssertFeedback(controller, PrototypeFeedbackKind.NextLevel, "NEXT:");
            Assert.IsFalse(previousButton.gameObject.activeSelf);
            Assert.IsTrue(replayButton.gameObject.activeSelf);
            Assert.IsTrue(nextUnlockedButton.gameObject.activeSelf);

            nextUnlockedButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(2, controller.CurrentLevelId);
            Assert.AreEqual(2, controller.SelectedLevel);
            Assert.IsTrue(previousButton.gameObject.activeSelf);
            Assert.IsTrue(replayButton.gameObject.activeSelf);
            Assert.IsFalse(nextUnlockedButton.gameObject.activeSelf);

            Assert.IsTrue(controller.TryPlaceNextTile(0, 0));
            yield return null;
            Assert.IsFalse(controller.GetTileAt(0, 0).IsEmpty);

            replayButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(2, controller.CurrentLevelId);
            Assert.AreEqual(2, controller.SelectedLevel);
            Assert.IsTrue(controller.GetTileAt(0, 0).IsEmpty);
            Assert.That(GetResultText().text, Does.Contain("Replay"));

            yield return LoadPrototypeScene();

            controller = Object.FindObjectOfType<PrototypeGameController>();
            Assert.NotNull(controller);
            Assert.AreEqual(2, controller.CurrentLevelId);
            Assert.AreEqual(2, controller.SelectedLevel);
            Assert.AreEqual(2, controller.HighestUnlockedLevel);

            previousButton = FindButton("PreviousLevelButton");
            replayButton = FindButton("ReplayLevelButton");
            nextUnlockedButton = FindButton("NextUnlockedLevelButton");
            Assert.IsTrue(previousButton.gameObject.activeSelf);
            Assert.IsTrue(replayButton.gameObject.activeSelf);
            Assert.IsFalse(nextUnlockedButton.gameObject.activeSelf);

            FindButton("ResetSaveButton").onClick.Invoke();
            yield return null;

            AssertNewPlayerState(controller);
            Assert.IsFalse(previousButton.gameObject.activeSelf);
            Assert.IsTrue(replayButton.gameObject.activeSelf);
            Assert.IsFalse(nextUnlockedButton.gameObject.activeSelf);
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
            Assert.That(GetResultText().text, Does.Contain("Goal:"));
            AssertFeedback(controller, PrototypeFeedbackKind.Retry, "RETRY:");
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
            AssertFeedback(controller, PrototypeFeedbackKind.Blocked, "BLOCKED:");

            yield return WinCurrentLevelAndClaim(controller);
            Assert.IsTrue(controller.StartNextLevel());
            yield return null;

            yield return WinCurrentLevelAndClaim(controller);
            Assert.AreEqual(120, controller.Coins);
            Assert.IsTrue(controller.CanAffordShelterUpgrade);
            Assert.That(GetResultText().text, Does.Contain("Upgrade available"));

            var previousCost = controller.ShelterUpgradeCost;
            upgradeButton.onClick.Invoke();
            yield return null;

            Assert.AreEqual(2, controller.ShelterUpgradeLevel);
            Assert.AreEqual(20, controller.Coins);
            Assert.Greater(controller.ShelterUpgradeCost, previousCost);
            Assert.That(GetResultText().text, Does.Contain("Shelter upgraded"));
            AssertFeedback(controller, PrototypeFeedbackKind.ShelterUpgrade, "UPGRADE:");
            Assert.That(GetShelterUpgradeText().text, Does.Contain("Lv 2"));

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

        private static void AssertFeedback(PrototypeGameController controller, PrototypeFeedbackKind kind, string prefix)
        {
            Assert.AreEqual(kind, controller.LastFeedbackKind);
            Assert.IsFalse(string.IsNullOrWhiteSpace(controller.LastFeedbackMessage));

            var hudView = Object.FindObjectOfType<PrototypeHudView>();
            Assert.NotNull(hudView);
            Assert.AreEqual(kind, hudView.CurrentFeedbackKind);
            Assert.AreEqual(kind, hudView.LastResultEffectKind);
            Assert.IsTrue(hudView.HasActiveResultEffect, $"{kind} should start a short result panel effect.");
            AssertResultPanelFlash(kind);
            AssertActiveButtonsRemainTappable();
            Assert.That(GetResultText().text, Does.StartWith(prefix));
        }

        private static void AssertVictoryCopyDoesNotContainDefeatLanguage(string result)
        {
            Assert.That(result, Does.Contain("Victory"));
            Assert.That(result, Does.Not.Contain("Defeat"));
            Assert.That(result, Does.Not.Contain("defeat"));
            Assert.That(result, Does.Not.Contain("needed more Wood defense"));
            Assert.That(result, Does.Not.Contain("attack power was too low"));
            Assert.That(result, Does.Not.Contain("had no recovery"));
            Assert.That(result, Does.Not.Contain("Emergency power was not charged"));
            Assert.That(result, Does.Not.Contain("board was blocked"));
            Assert.That(result, Does.Not.Contain("overwhelmed"));
            Assert.That(result, Does.Not.Contain("Try merging more Wood tiles"));
            Assert.That(result, Does.Not.Contain("Merge Metal tiles"));
            Assert.That(result, Does.Not.Contain("Add Food tiles"));
            Assert.That(result, Does.Not.Contain("Merge Energy tiles"));
            Assert.That(result, Does.Not.Contain(PrototypeBoardEvaluator.WeakWall));
            Assert.That(result, Does.Not.Contain(PrototypeBoardEvaluator.LowAttack));
            Assert.That(result, Does.Not.Contain(PrototypeBoardEvaluator.NoHeal));
            Assert.That(result, Does.Not.Contain(PrototypeBoardEvaluator.NoEnergy));
            Assert.That(result, Does.Not.Contain(PrototypeBoardEvaluator.BoardBlocked));
            Assert.That(result, Does.Not.Contain(PrototypeBoardEvaluator.Overwhelmed));
        }

        private static void AssertDefeatCopyHasActionableHint(string result)
        {
            Assert.IsTrue(
                result.Contains("Try merging more Wood tiles") ||
                result.Contains("Merge Metal tiles") ||
                result.Contains("Add Food tiles") ||
                result.Contains("Merge Energy tiles") ||
                result.Contains("Leave empty spaces") ||
                result.Contains("Upgrade your shelter") ||
                result.Contains("Build a balanced board"),
                $"Defeat copy should include an actionable hint. Result: {result}");
            Assert.That(result, Does.Not.Contain(PrototypeBoardEvaluator.WeakWall));
            Assert.That(result, Does.Not.Contain(PrototypeBoardEvaluator.LowAttack));
            Assert.That(result, Does.Not.Contain(PrototypeBoardEvaluator.NoHeal));
            Assert.That(result, Does.Not.Contain(PrototypeBoardEvaluator.NoEnergy));
            Assert.That(result, Does.Not.Contain(PrototypeBoardEvaluator.BoardBlocked));
            Assert.That(result, Does.Not.Contain(PrototypeBoardEvaluator.Overwhelmed));
        }

        private static Button FindCellButton(int x, int y)
        {
            var button = GameObject.Find($"Cell_{x}_{y}")?.GetComponent<Button>();
            Assert.NotNull(button);
            return button;
        }

        private static Image GetCellImage(int x, int y)
        {
            var image = GameObject.Find($"Cell_{x}_{y}")?.GetComponent<Image>();
            Assert.NotNull(image);
            return image;
        }

        private static float GetCellScale(int x, int y)
        {
            var button = FindCellButton(x, y);
            return button.transform.localScale.x;
        }

        private static Image GetButtonImage(string name)
        {
            var image = FindButton(name)?.GetComponent<Image>();
            Assert.NotNull(image, $"{name} should have a button image.");
            return image;
        }

        private static Text GetTutorialText()
        {
            var tutorialText = GameObject.Find("TutorialText")?.GetComponent<Text>();
            Assert.NotNull(tutorialText);
            return tutorialText;
        }

        private static Text GetHudText(string name)
        {
            var text = GameObject.Find(name)?.GetComponent<Text>();
            Assert.NotNull(text, $"{name} should exist.");
            return text;
        }

        private static void AssertRequiredPhoneUiExists()
        {
            foreach (var textName in HudTextNames)
                Assert.NotNull(GetHudText(textName));

            Assert.NotNull(GameObject.Find("BoardGrid")?.GetComponent<RectTransform>());
            AssertBoardPanelStyle();
            AssertActionPanelStyle();
            AssertResultPanelExists();
            AssertHudSectionLabelStyles();
            AssertHudPresentationPanelsExist();
            AssertOpaqueCanvasBackground();

            foreach (var buttonName in ActionButtonNames)
                Assert.NotNull(FindButton(buttonName), $"{buttonName} should exist.");
        }

        private static void AssertHudPresentationPanelsExist()
        {
            foreach (var panelName in HudPanelNames)
                Assert.NotNull(GameObject.Find(panelName), $"{panelName} should exist.");

            AssertHudPanelStyle(PrototypeHudView.HeaderPanelName, PrototypeVisualKit.HudPanelBackground);
            AssertHudPanelStyle(PrototypeHudView.ShelterPanelName, PrototypeVisualKit.HudPanelSecondaryBackground);
            AssertHudPanelStyle(PrototypeHudView.RewardsPanelName, PrototypeVisualKit.HudPanelSecondaryBackground);
            AssertHudPanelStyle(PrototypeHudView.QuestsPanelName, PrototypeVisualKit.HudPanelBackground);
            AssertHudPanelStyle(PrototypeHudView.BoardInfoPanelName, PrototypeVisualKit.HudPanelSecondaryBackground);
        }

        private static void AssertHudPanelStyle(string panelName, Color expectedColor)
        {
            var panel = GameObject.Find(panelName);
            Assert.NotNull(panel, $"{panelName} should exist.");

            var image = panel.GetComponent<Image>();
            Assert.NotNull(image, $"{panelName} should have a visual background.");
            Assert.IsFalse(image.raycastTarget, $"{panelName} should not block gameplay input.");
            AssertColorApproximately(expectedColor, image.color);

            var outline = panel.GetComponent<Outline>();
            Assert.NotNull(outline, $"{panelName} should have a visible border.");
            AssertColorApproximately(PrototypeVisualKit.HudPanelAccent, outline.effectColor);
            AssertPlainShadow(panel, $"{panelName} should have subtle depth.");
        }

        private static void AssertBoardPanelStyle()
        {
            var boardGrid = GameObject.Find("BoardGrid");
            Assert.NotNull(boardGrid);

            var image = boardGrid.GetComponent<Image>();
            Assert.NotNull(image, "BoardGrid should have a visual panel background.");
            Assert.IsFalse(image.raycastTarget);
            AssertColorApproximately(PrototypeVisualKit.BoardPanelBackground, image.color);

            var outline = boardGrid.GetComponent<Outline>();
            Assert.NotNull(outline, "BoardGrid should have a visible border.");
            AssertColorApproximately(PrototypeVisualKit.BoardFrameAccent, outline.effectColor);
            Assert.That(outline.effectDistance.x, Is.EqualTo(5f).Within(0.01f));
            Assert.That(outline.effectDistance.y, Is.EqualTo(-5f).Within(0.01f));
            AssertPlainShadow(boardGrid, "BoardGrid should have subtle depth.");

            var rectTransform = boardGrid.GetComponent<RectTransform>();
            Assert.NotNull(rectTransform);
            Assert.Greater(rectTransform.sizeDelta.x, 390f, "Board frame should include visible padding around cells.");
            Assert.Greater(rectTransform.sizeDelta.y, 390f, "Board frame should include visible padding around cells.");
        }

        private static void AssertCellUsesVisualBorder(int x, int y)
        {
            var cell = GameObject.Find($"Cell_{x}_{y}");
            Assert.NotNull(cell);

            var outline = cell.GetComponent<Outline>();
            Assert.NotNull(outline, $"Cell_{x}_{y} should have a visible border.");
            AssertColorApproximately(PrototypeVisualKit.CellBorder, outline.effectColor);
        }

        private static void AssertFilledCellUsesHighlightBorder(int x, int y)
        {
            var cell = GameObject.Find($"Cell_{x}_{y}");
            Assert.NotNull(cell);

            var outline = cell.GetComponent<Outline>();
            Assert.NotNull(outline, $"Cell_{x}_{y} should have a filled-tile border.");
            AssertColorApproximately(PrototypeVisualKit.CellHighlightBorder, outline.effectColor);
        }

        private static void AssertActionPanelStyle()
        {
            var panel = GameObject.Find(PrototypeBoardView.ActionPanelName);
            Assert.NotNull(panel, "Action buttons should sit on a grouped visual panel.");

            var image = panel.GetComponent<Image>();
            Assert.NotNull(image);
            Assert.IsFalse(image.raycastTarget);
            AssertColorApproximately(PrototypeVisualKit.ActionPanelBackground, image.color);

            var outline = panel.GetComponent<Outline>();
            Assert.NotNull(outline, "Action panel should have a visible border.");
            AssertColorApproximately(PrototypeVisualKit.HudPanelAccent, outline.effectColor);
            AssertPlainShadow(panel, "Action panel should have subtle depth.");

            var rectTransform = panel.GetComponent<RectTransform>();
            Assert.NotNull(rectTransform);
            Assert.Greater(rectTransform.sizeDelta.x, 650f);
            Assert.Greater(rectTransform.sizeDelta.y, 180f);
        }

        private static void AssertResultPanelExists()
        {
            var panel = GameObject.Find(PrototypeHudView.ResultPanelName);
            Assert.NotNull(panel, "Result/status text should be visually bounded.");

            var image = panel.GetComponent<Image>();
            Assert.NotNull(image);
            Assert.IsFalse(image.raycastTarget);
            AssertPlainShadow(panel, "Result panel should have subtle depth.");

            var hudView = Object.FindObjectOfType<PrototypeHudView>();
            Assert.NotNull(hudView);
            var expectedColor = hudView.HasActiveResultEffect
                ? PrototypeVisualKit.GetResultPanelFlashColor(hudView.CurrentFeedbackKind)
                : PrototypeVisualKit.GetResultPanelColor(hudView.CurrentFeedbackKind);
            AssertColorApproximately(expectedColor, image.color);
        }

        private static void AssertResultPanelFlash(PrototypeFeedbackKind kind)
        {
            var panelImage = GameObject.Find(PrototypeHudView.ResultPanelName)?.GetComponent<Image>();
            Assert.NotNull(panelImage);
            AssertColorApproximately(PrototypeVisualKit.GetResultPanelFlashColor(kind), panelImage.color);
        }

        private static void AssertResultPanelNormal(PrototypeFeedbackKind kind)
        {
            var panelImage = GameObject.Find(PrototypeHudView.ResultPanelName)?.GetComponent<Image>();
            Assert.NotNull(panelImage);
            AssertColorApproximately(PrototypeVisualKit.GetResultPanelColor(kind), panelImage.color);
        }

        private static void AssertActiveButtonsRemainTappable()
        {
            foreach (var buttonName in ActionButtonNames)
            {
                var button = FindButton(buttonName);
                if (button != null && button.gameObject.activeInHierarchy)
                    Assert.IsTrue(button.interactable, $"{buttonName} should remain tappable while feedback effects are active.");
            }
        }

        private static void AssertResultPanelBounds(Canvas canvas, Rect resultRect)
        {
            AssertResultPanelExists();

            var panelRectTransform = GameObject.Find(PrototypeHudView.ResultPanelName)?.GetComponent<RectTransform>();
            Assert.NotNull(panelRectTransform);
            var panelRect = GetCanvasRect(canvas, panelRectTransform);
            Assert.IsTrue(
                RectContains(panelRect, resultRect),
                $"Result/status panel should contain ResultText. panel={FormatRect(panelRect)} result={FormatRect(resultRect)}");
        }

        private static void AssertHudSectionLabelStyles()
        {
            AssertSectionLabelStyle(PrototypeHudView.ShelterSectionLabelName);
            AssertSectionLabelStyle(PrototypeHudView.BoardSectionLabelName);
            AssertSectionLabelStyle(PrototypeHudView.ActionsSectionLabelName);
            AssertSectionLabelStyle(PrototypeHudView.RewardsSectionLabelName);
            AssertSectionLabelStyle(PrototypeHudView.QuestsSectionLabelName);
        }

        private static void AssertSectionLabelStyle(string name)
        {
            var text = GetHudText(name);
            AssertColorApproximately(PrototypeVisualKit.SectionText, text.color);
            Assert.AreEqual(FontStyle.Bold, text.fontStyle);
        }

        private static void AssertOpaqueCanvasBackground()
        {
            var background = GameObject.Find(PrototypeHudView.OpaqueBackgroundName);
            Assert.NotNull(background, "Canvas should have an opaque background to clear Android text redraws.");

            var backgroundImage = background.GetComponent<Image>();
            Assert.NotNull(backgroundImage);
            Assert.IsFalse(backgroundImage.raycastTarget);
            Assert.GreaterOrEqual(backgroundImage.color.a, 0.99f);
            AssertColorApproximately(PrototypeVisualKit.CanvasBackground, backgroundImage.color);

            var rectTransform = background.GetComponent<RectTransform>();
            Assert.NotNull(rectTransform);
            Assert.AreEqual(Vector2.zero, rectTransform.anchorMin);
            Assert.AreEqual(Vector2.one, rectTransform.anchorMax);
            Assert.AreEqual(0, background.transform.GetSiblingIndex());
        }

        private static void AssertVisibleActionButton(string name)
        {
            var button = FindButton(name);
            Assert.NotNull(button, $"{name} should exist.");
            Assert.IsTrue(button.gameObject.activeInHierarchy, $"{name} should be visible.");
            Assert.IsTrue(button.interactable, $"{name} should be tappable.");
        }

        private static void AssertPrimaryActionButton(string name)
        {
            AssertVisibleActionButton(name);
            var button = FindButton(name);
            var rectTransform = (RectTransform)button.transform;
            Assert.GreaterOrEqual(rectTransform.sizeDelta.x, 218f, $"{name} should be visually emphasized as the primary next action.");
            Assert.GreaterOrEqual(rectTransform.sizeDelta.y, 50f, $"{name} should be visually emphasized as the primary next action.");
            AssertColorApproximately(PrototypeVisualKit.GetActionButtonColor(name, true), GetButtonImage(name).color);

            var outline = button.GetComponent<Outline>();
            Assert.NotNull(outline, $"{name} should keep a visible primary outline.");
            AssertColorApproximately(PrototypeVisualKit.PrimaryText, outline.effectColor);
            Assert.Greater(
                ColorDistance(PrototypeVisualKit.GetActionButtonColor(name, true), PrototypeVisualKit.GetActionButtonColor("ReplayLevelButton", false)),
                0.28f,
                $"{name} should read stronger than secondary navigation.");
        }

        private static void AssertPhoneSafeLayout()
        {
            Canvas.ForceUpdateCanvases();

            var canvas = Object.FindObjectOfType<Canvas>();
            Assert.NotNull(canvas);

            var boardRectTransform = GameObject.Find("BoardGrid")?.GetComponent<RectTransform>();
            Assert.NotNull(boardRectTransform);
            var boardRect = GetCanvasRect(canvas, boardRectTransform);
            var canvasRect = ((RectTransform)canvas.transform).rect;
            Assert.Greater(boardRect.width, 300f);
            Assert.Greater(boardRect.height, 300f);
            Assert.Less(Mathf.Abs(boardRect.center.x - canvasRect.center.x), 2f, "Board should stay centered horizontally.");

            var hudRects = new Dictionary<string, Rect>();
            foreach (var textName in HudTextNames)
            {
                var text = GetHudText(textName);
                var rect = GetCanvasRect(canvas, (RectTransform)text.transform);
                Assert.Greater(rect.width, 100f, $"{textName} should have bounded width.");
                var minimumHeight = IsCompactHudText(textName) ? 10f : 20f;
                Assert.Greater(rect.height, minimumHeight, $"{textName} should have bounded height.");
                Assert.AreEqual(HorizontalWrapMode.Wrap, text.horizontalOverflow, $"{textName} should wrap horizontally.");
                Assert.AreEqual(VerticalWrapMode.Truncate, text.verticalOverflow, $"{textName} should clamp vertically.");
                Assert.IsFalse(text.resizeTextForBestFit, $"{textName} should avoid Best Fit line collapse on Android.");
                hudRects[textName] = rect;
            }

            for (var i = 0; i < HudTextNames.Length; i++)
            {
                for (var j = i + 1; j < HudTextNames.Length; j++)
                {
                    var firstName = HudTextNames[i];
                    var secondName = HudTextNames[j];
                    Assert.IsFalse(
                        RectsOverlap(hudRects[firstName], hudRects[secondName]),
                        $"{firstName} overlaps {secondName}.");
                }
            }

            foreach (var textName in HudTextNames)
            {
                Assert.IsFalse(
                    RectsOverlap(boardRect, hudRects[textName]),
                    $"BoardGrid overlaps {textName}.");
            }

            AssertHudSectionLabelStyles();
            AssertHudPresentationPanels(canvas, hudRects);
            AssertResultPanelBounds(canvas, hudRects["ResultText"]);
            AssertActionPanelStyle();
            AssertActiveActionButtonsDoNotOverlap(canvas, hudRects["ResultText"]);
        }

        private static void AssertHudPresentationPanels(Canvas canvas, Dictionary<string, Rect> hudRects)
        {
            AssertHudPresentationPanelContains(
                canvas,
                hudRects,
                PrototypeHudView.HeaderPanelName,
                PrototypeVisualKit.HudPanelBackground,
                "LevelText",
                PrototypeHudView.ObjectiveTextName,
                "TutorialText");
            AssertHudPresentationPanelContains(
                canvas,
                hudRects,
                PrototypeHudView.ShelterPanelName,
                PrototypeVisualKit.HudPanelSecondaryBackground,
                PrototypeHudView.ShelterSectionLabelName,
                "ShelterHpText",
                PrototypeHudView.ShelterUpgradeTextName);
            AssertHudPresentationPanelContains(
                canvas,
                hudRects,
                PrototypeHudView.RewardsPanelName,
                PrototypeVisualKit.HudPanelSecondaryBackground,
                PrototypeHudView.RewardsSectionLabelName,
                "WalletText",
                PrototypeHudView.RewardTextName);
            AssertHudPresentationPanelContains(
                canvas,
                hudRects,
                PrototypeHudView.QuestsPanelName,
                PrototypeVisualKit.HudPanelBackground,
                PrototypeHudView.QuestsSectionLabelName,
                PrototypeHudView.QuestTextName);
            AssertHudPresentationPanelContains(
                canvas,
                hudRects,
                PrototypeHudView.BoardInfoPanelName,
                PrototypeVisualKit.HudPanelSecondaryBackground,
                PrototypeHudView.BoardSectionLabelName,
                "NextTileText",
                PrototypeHudView.WaveRosterTextName);
        }

        private static void AssertHudPresentationPanelContains(
            Canvas canvas,
            Dictionary<string, Rect> hudRects,
            string panelName,
            Color expectedColor,
            params string[] textNames)
        {
            AssertHudPanelStyle(panelName, expectedColor);

            var panelRectTransform = GameObject.Find(panelName)?.GetComponent<RectTransform>();
            Assert.NotNull(panelRectTransform, $"{panelName} should have a rect transform.");
            var panelRect = GetCanvasRect(canvas, panelRectTransform);

            foreach (var textName in textNames)
            {
                Assert.IsTrue(hudRects.TryGetValue(textName, out var textRect), $"{textName} should be tracked in HUD layout.");
                Assert.IsTrue(
                    RectContains(panelRect, textRect),
                    $"{panelName} should contain {textName}. panel={FormatRect(panelRect)} text={FormatRect(textRect)}");
            }
        }

        private static bool IsCompactHudText(string textName)
        {
            return textName == PrototypeHudView.ObjectiveTextName ||
                   textName == PrototypeHudView.WaveRosterTextName ||
                   textName == PrototypeHudView.ShelterSectionLabelName ||
                   textName == PrototypeHudView.BoardSectionLabelName ||
                   textName == PrototypeHudView.ActionsSectionLabelName ||
                   textName == PrototypeHudView.RewardsSectionLabelName ||
                   textName == PrototypeHudView.QuestsSectionLabelName ||
                   textName == "NextTileText";
        }

        private static void AssertActiveActionButtonsDoNotOverlap(Canvas canvas, Rect resultRect)
        {
            var activeButtons = new List<Button>();
            foreach (var buttonName in ActionButtonNames)
            {
                var button = FindButton(buttonName);
                if (button != null && button.gameObject.activeInHierarchy)
                    activeButtons.Add(button);
            }

            for (var i = 0; i < activeButtons.Count; i++)
            {
                var first = activeButtons[i];
                Assert.IsTrue(first.interactable, $"{first.name} should be tappable.");
                var isPrimary = IsExpectedPrimaryAction(first.name);
                AssertColorApproximately(PrototypeVisualKit.GetActionButtonColor(first.name, isPrimary), GetButtonImage(first.name).color);

                var outline = first.GetComponent<Outline>();
                Assert.NotNull(outline, $"{first.name} should keep a role border.");
                AssertColorApproximately(
                    isPrimary ? PrototypeVisualKit.PrimaryText : PrototypeVisualKit.ButtonBorder,
                    outline.effectColor);
                AssertPlainShadow(first.gameObject, $"{first.name} should keep subtle button depth.");

                var firstRect = GetCanvasRect(canvas, (RectTransform)first.transform);
                Assert.Greater(firstRect.width, 120f, $"{first.name} should keep a tappable width.");
                Assert.Greater(firstRect.height, 36f, $"{first.name} should keep a tappable height.");
                Assert.IsFalse(RectsOverlap(firstRect, resultRect), $"{first.name} overlaps ResultText.");

                for (var j = i + 1; j < activeButtons.Count; j++)
                {
                    var second = activeButtons[j];
                    var secondRect = GetCanvasRect(canvas, (RectTransform)second.transform);
                    Assert.IsFalse(
                        RectsOverlap(firstRect, secondRect),
                        $"{first.name} overlaps {second.name}.");
                }
            }
        }

        private static bool IsExpectedPrimaryAction(string buttonName)
        {
            return buttonName == "StartWaveButton" ||
                   buttonName == "ClaimRewardButton" ||
                   buttonName == "NextLevelButton" ||
                   buttonName == "RetryButton" ||
                   buttonName == "ReviveButton";
        }

        private static Rect GetCanvasRect(Canvas canvas, RectTransform rectTransform)
        {
            var corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            var canvasTransform = canvas.transform;
            for (var i = 0; i < corners.Length; i++)
                corners[i] = canvasTransform.InverseTransformPoint(corners[i]);

            var minX = corners[0].x;
            var maxX = corners[0].x;
            var minY = corners[0].y;
            var maxY = corners[0].y;

            for (var i = 1; i < corners.Length; i++)
            {
                minX = Mathf.Min(minX, corners[i].x);
                maxX = Mathf.Max(maxX, corners[i].x);
                minY = Mathf.Min(minY, corners[i].y);
                maxY = Mathf.Max(maxY, corners[i].y);
            }

            return Rect.MinMaxRect(minX, minY, maxX, maxY);
        }

        private static bool RectsOverlap(Rect first, Rect second)
        {
            const float tolerance = 1f;
            return first.xMin < second.xMax - tolerance &&
                   first.xMax > second.xMin + tolerance &&
                   first.yMin < second.yMax - tolerance &&
                   first.yMax > second.yMin + tolerance;
        }

        private static bool RectContains(Rect outer, Rect inner)
        {
            const float tolerance = 1f;
            return outer.xMin <= inner.xMin + tolerance &&
                   outer.xMax >= inner.xMax - tolerance &&
                   outer.yMin <= inner.yMin + tolerance &&
                   outer.yMax >= inner.yMax - tolerance;
        }

        private static string FormatRect(Rect rect)
        {
            return $"({rect.xMin:F1},{rect.yMin:F1})-({rect.xMax:F1},{rect.yMax:F1})";
        }

        private static void AssertPlainShadow(GameObject target, string message)
        {
            var shadows = target.GetComponents<Shadow>();
            foreach (var shadow in shadows)
            {
                if (shadow.GetType() != typeof(Shadow))
                    continue;

                AssertColorApproximately(PrototypeVisualKit.PanelShadow, shadow.effectColor);
                Assert.IsFalse(shadow.useGraphicAlpha);
                return;
            }

            Assert.Fail(message);
        }

        private static void AssertQuestTextIsCompact()
        {
            var questText = GetQuestText().text;
            var lines = questText.Split('\n');
            Assert.LessOrEqual(lines.Length, 3, "Quest section should stay to one short line per quest.");

            foreach (var line in lines)
                Assert.LessOrEqual(line.Length, 48, $"Quest line should stay short for Android portrait: {line}");
        }

        private static void AssertColorApproximately(Color expected, Color actual)
        {
            const float tolerance = 0.01f;
            Assert.That(actual.r, Is.EqualTo(expected.r).Within(tolerance));
            Assert.That(actual.g, Is.EqualTo(expected.g).Within(tolerance));
            Assert.That(actual.b, Is.EqualTo(expected.b).Within(tolerance));
            Assert.That(actual.a, Is.EqualTo(expected.a).Within(tolerance));
        }

        private static float ColorDistance(Color first, Color second)
        {
            var red = first.r - second.r;
            var green = first.g - second.g;
            var blue = first.b - second.b;
            return Mathf.Sqrt(red * red + green * green + blue * blue);
        }

        private static Text GetWalletText()
        {
            var walletText = GameObject.Find("WalletText")?.GetComponent<Text>();
            Assert.NotNull(walletText);
            return walletText;
        }

        private static Text GetRewardText()
        {
            var rewardText = GameObject.Find(PrototypeHudView.RewardTextName)?.GetComponent<Text>();
            Assert.NotNull(rewardText);
            return rewardText;
        }

        private static Text GetQuestText()
        {
            var questText = GameObject.Find(PrototypeHudView.QuestTextName)?.GetComponent<Text>();
            Assert.NotNull(questText);
            return questText;
        }

        private static Text GetShelterHpText()
        {
            var shelterHpText = GameObject.Find("ShelterHpText")?.GetComponent<Text>();
            Assert.NotNull(shelterHpText);
            return shelterHpText;
        }

        private static Text GetShelterUpgradeText()
        {
            var shelterUpgradeText = GameObject.Find(PrototypeHudView.ShelterUpgradeTextName)?.GetComponent<Text>();
            Assert.NotNull(shelterUpgradeText);
            return shelterUpgradeText;
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
