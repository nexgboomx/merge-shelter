using System;
using System.Collections.Generic;
using System.Linq;
using MergeShelter.Board;
using MergeShelter.Combat;
using MergeShelter.Levels;
using NUnit.Framework;

namespace MergeShelter.Tests.EditMode
{
    public sealed class SprintOneLevelCatalogTests
    {
        [Test]
        public void Catalog_HasThirtySequentialCompleteLevels()
        {
            var levels = SprintOneLevelCatalog.CreateLevels();

            Assert.AreEqual(30, levels.Count);
            for (var i = 0; i < levels.Count; i++)
            {
                var level = levels[i];
                Assert.AreEqual(i + 1, level.LevelId);
                Assert.IsFalse(string.IsNullOrWhiteSpace(level.DisplayName), $"Level {level.LevelId} needs a display name.");
                Assert.IsFalse(string.IsNullOrWhiteSpace(level.TutorialMessage), $"Level {level.LevelId} needs a status message.");
                Assert.IsNotEmpty(level.AvailableTiles, $"Level {level.LevelId} needs available tiles.");
                Assert.IsNotEmpty(level.Enemies, $"Level {level.LevelId} needs enemies.");
                Assert.Greater(level.CoinReward, 0, $"Level {level.LevelId} needs a coin reward.");
                Assert.GreaterOrEqual(level.PartsReward, 0, $"Level {level.LevelId} needs a parts reward.");
            }
        }

        [Test]
        public void Rewards_GenerallyIncreaseAcrossSprintSixBands()
        {
            var levels = SprintOneLevelCatalog.CreateLevels();

            AssertBandAverageIncreases(levels, 1, 5, 6, 10, level => level.CoinReward, "coin rewards");
            AssertBandAverageIncreases(levels, 6, 10, 11, 20, level => level.CoinReward, "coin rewards");
            AssertBandAverageIncreases(levels, 11, 20, 21, 30, level => level.CoinReward, "coin rewards");

            AssertBandAverageIncreases(levels, 1, 5, 6, 10, level => level.PartsReward, "parts rewards");
            AssertBandAverageIncreases(levels, 6, 10, 11, 20, level => level.PartsReward, "parts rewards");
            AssertBandAverageIncreases(levels, 11, 20, 21, 30, level => level.PartsReward, "parts rewards");
        }

        [Test]
        public void EnemyPressure_GenerallyIncreasesAcrossSprintSixBands()
        {
            var levels = SprintOneLevelCatalog.CreateLevels();

            AssertBandAverageIncreases(levels, 1, 5, 6, 10, level => EvaluateEmptyBoard(level).EnemyPressure, "enemy pressure");
            AssertBandAverageIncreases(levels, 6, 10, 11, 20, level => EvaluateEmptyBoard(level).EnemyPressure, "enemy pressure");
            AssertBandAverageIncreases(levels, 11, 20, 21, 30, level => EvaluateEmptyBoard(level).EnemyPressure, "enemy pressure");
        }

        [Test]
        public void LevelOne_RemainsFirstRunTutorialEntry()
        {
            var level = SprintOneLevelCatalog.CreateLevels()[0];

            Assert.AreEqual(1, level.LevelId);
            Assert.AreEqual("First Night", level.DisplayName);
            CollectionAssert.AreEqual(new[] { TileType.Wood }, level.AvailableTiles);
            Assert.That(level.TutorialMessage, Does.Contain("Wood"));
            Assert.Less(EvaluateEmptyBoard(level).EnemyPressure, 50);
        }

        [Test]
        public void LevelTen_RemainsUsefulForRetryAndReviveRegression()
        {
            var level = SprintOneLevelCatalog.CreateLevels()[9];

            Assert.AreEqual(10, level.LevelId);
            Assert.AreEqual(250, level.CoinReward);
            Assert.AreEqual(5, level.PartsReward);

            var emptyBoardResult = Evaluate(new BoardModel(), level);
            Assert.GreaterOrEqual(emptyBoardResult.NetDamage, 100);
            Assert.AreEqual(PrototypeBoardEvaluator.WeakWall, emptyBoardResult.FailReason);

            var strongBoardResult = Evaluate(CreateStrongTierThreeBoard(), level);
            Assert.AreEqual(0, strongBoardResult.NetDamage);
            Assert.That(strongBoardResult.ResultExplanation, Does.Contain("Victory"));
        }

        [Test]
        public void LevelThirty_ExistsAndIsBeatableByStrongBoardAssumption()
        {
            var level = SprintOneLevelCatalog.CreateLevels()[29];

            Assert.AreEqual(30, level.LevelId);
            Assert.AreEqual("Prototype Stand", level.DisplayName);
            Assert.Greater(level.CoinReward, 0);
            Assert.Greater(level.PartsReward, 0);

            var emptyBoardResult = EvaluateEmptyBoard(level);
            Assert.Greater(emptyBoardResult.NetDamage, 100);

            var strongBoardResult = Evaluate(CreateStrongLevelThirtyBoard(), level);
            Assert.AreEqual(0, strongBoardResult.NetDamage);
            Assert.That(strongBoardResult.ResultExplanation, Does.Contain("Victory"));
        }

        private static void AssertBandAverageIncreases(
            IReadOnlyList<LevelDefinition> levels,
            int firstBandStart,
            int firstBandEnd,
            int secondBandStart,
            int secondBandEnd,
            Func<LevelDefinition, int> selector,
            string label)
        {
            var firstAverage = Average(levels, firstBandStart, firstBandEnd, selector);
            var secondAverage = Average(levels, secondBandStart, secondBandEnd, selector);
            Assert.Greater(secondAverage, firstAverage, $"{label} should increase from Levels {firstBandStart}-{firstBandEnd} to Levels {secondBandStart}-{secondBandEnd}.");
        }

        private static double Average(
            IEnumerable<LevelDefinition> levels,
            int startLevel,
            int endLevel,
            Func<LevelDefinition, int> selector)
        {
            return levels
                .Where(level => level.LevelId >= startLevel && level.LevelId <= endLevel)
                .Average(selector);
        }

        private static PrototypeBoardEvaluationResult EvaluateEmptyBoard(LevelDefinition level)
        {
            return Evaluate(new BoardModel(), level);
        }

        private static PrototypeBoardEvaluationResult Evaluate(BoardModel board, LevelDefinition level)
        {
            return new PrototypeBoardEvaluator().Evaluate(board, level.Enemies);
        }

        private static BoardModel CreateStrongTierThreeBoard()
        {
            var board = new BoardModel();
            board.SetTile(new BoardPosition(0, 0), new TileData(TileType.Wood, 3));
            board.SetTile(new BoardPosition(1, 0), new TileData(TileType.Metal, 3));
            board.SetTile(new BoardPosition(2, 0), new TileData(TileType.Food, 3));
            board.SetTile(new BoardPosition(3, 0), new TileData(TileType.Energy, 3));
            return board;
        }

        private static BoardModel CreateStrongLevelThirtyBoard()
        {
            var board = new BoardModel();
            board.SetTile(new BoardPosition(0, 0), new TileData(TileType.Wood, 4));
            board.SetTile(new BoardPosition(1, 0), new TileData(TileType.Metal, 4));
            board.SetTile(new BoardPosition(2, 0), new TileData(TileType.Food, 4));
            board.SetTile(new BoardPosition(3, 0), new TileData(TileType.Energy, 4));
            return board;
        }
    }
}
