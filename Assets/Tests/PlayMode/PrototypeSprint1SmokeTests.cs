using System.Collections;
using System.Reflection;
using MergeShelter.Board;
using MergeShelter.Core;
using MergeShelter.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace MergeShelter.Tests.PlayMode
{
    public sealed class PrototypeSprint1SmokeTests
    {
        private const string SceneName = "PrototypeSprint1";

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

        private static void SetStrongLevelOneBoard(PrototypeGameController controller)
        {
            var board = GetBoard(controller);
            board.SetTile(new BoardPosition(0, 0), new TileData(TileType.Wood, 3));
        }
    }
}
