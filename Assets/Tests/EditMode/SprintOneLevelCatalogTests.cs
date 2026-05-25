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

            var strongBoardResult = Evaluate(CreateLateStrongBoard(), level);
            Assert.AreEqual(0, strongBoardResult.NetDamage);
            Assert.That(strongBoardResult.ResultExplanation, Does.Contain("Victory"));
        }

        [Test]
        public void BalanceSimulation_EmptyBoardFailsMostLevelsAndAllLateLevels()
        {
            var results = Simulate(CreateEmptyBoard);

            Assert.GreaterOrEqual(FailCount(results, 1, 30), 25);
            Assert.AreEqual(10, FailCount(results, 21, 30), "Empty boards should fail every late prototype level.");
        }

        [Test]
        public void BalanceSimulation_WeakBoardOnlyPassesForgivingEarlyLevels()
        {
            var results = Simulate(CreateWeakBoard);

            Assert.IsTrue(WinningLevels(results).Contains(1));
            Assert.IsTrue(WinningLevels(results).All(levelId => levelId <= 5), "Weak boards should only win forgiving early levels.");
            Assert.GreaterOrEqual(FailCount(results, 6, 30), 23);
        }

        [Test]
        public void BalanceSimulation_MediumBoardPassesEarlyAndManyMidLevelsButIsStressedLate()
        {
            var results = Simulate(CreateMediumBoard);

            Assert.AreEqual(10, WinCount(results, 1, 10), "Medium boards should clear the early learning band.");
            Assert.GreaterOrEqual(WinCount(results, 11, 20), 7, "Medium boards should pass many resource planning levels.");
            Assert.LessOrEqual(WinCount(results, 21, 30), 3, "Medium boards should be visibly stressed in late prototype levels.");
            Assert.IsTrue(GetResult(results, 30).NetDamage > 0, "Medium boards should not trivialize the endpoint check.");
        }

        [Test]
        public void BalanceSimulation_StrongBoardPassesMostLevels()
        {
            var results = Simulate(CreateStrongTierThreeBoard);

            Assert.GreaterOrEqual(WinCount(results, 1, 30), 24);
            Assert.GreaterOrEqual(FailCount(results, 21, 30), 1, "Late prototype levels should still pressure a strong board.");
        }

        [Test]
        public void BalanceSimulation_LateStrongBoardPassesLevelThirty()
        {
            var result = Evaluate(CreateLateStrongBoard(), SprintOneLevelCatalog.CreateLevels()[29]);

            Assert.AreEqual(0, result.NetDamage);
            Assert.That(result.ResultExplanation, Does.Contain("Victory"));
        }

        [Test]
        public void BalanceSimulation_ResultExplanationsAreUsefulForWinsAndFailures()
        {
            var assumptions = new[]
            {
                new BoardAssumption("empty", CreateEmptyBoard),
                new BoardAssumption("weak", CreateWeakBoard),
                new BoardAssumption("medium", CreateMediumBoard),
                new BoardAssumption("strong", CreateStrongTierThreeBoard),
                new BoardAssumption("late_strong", CreateLateStrongBoard)
            };

            foreach (var assumption in assumptions)
            {
                foreach (var result in Simulate(assumption.CreateBoard))
                {
                    Assert.IsFalse(
                        string.IsNullOrWhiteSpace(result.Result.ResultExplanation),
                        $"{assumption.Name} board Level {result.Level.LevelId} should explain the result.");

                    if (result.Result.NetDamage > 0)
                    {
                        Assert.Contains(result.Result.FailReason, UsefulFailReasons);
                        Assert.That(
                            result.Result.ResultExplanation,
                            Does.Contain("Defeat"),
                            $"{assumption.Name} board Level {result.Level.LevelId} should explain defeat.");
                    }
                    else
                    {
                        Assert.IsTrue(string.IsNullOrEmpty(result.Result.FailReason));
                        Assert.That(
                            result.Result.ResultExplanation,
                            Does.Contain("Victory"),
                            $"{assumption.Name} board Level {result.Level.LevelId} should explain victory.");
                    }
                }
            }
        }

        private static readonly string[] UsefulFailReasons =
        {
            PrototypeBoardEvaluator.WeakWall,
            PrototypeBoardEvaluator.LowAttack,
            PrototypeBoardEvaluator.NoHeal,
            PrototypeBoardEvaluator.NoEnergy,
            PrototypeBoardEvaluator.BoardBlocked,
            PrototypeBoardEvaluator.Overwhelmed,
            PrototypeBoardEvaluator.Unknown
        };

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

        private static IReadOnlyList<BalanceSimulationResult> Simulate(Func<BoardModel> createBoard)
        {
            return SprintOneLevelCatalog.CreateLevels()
                .Select(level => new BalanceSimulationResult(level, Evaluate(createBoard(), level)))
                .ToList();
        }

        private static IReadOnlyList<int> WinningLevels(IReadOnlyList<BalanceSimulationResult> results)
        {
            return results
                .Where(result => result.Result.NetDamage <= 0)
                .Select(result => result.Level.LevelId)
                .ToList();
        }

        private static int WinCount(IReadOnlyList<BalanceSimulationResult> results, int startLevel, int endLevel)
        {
            return results.Count(result =>
                result.Level.LevelId >= startLevel &&
                result.Level.LevelId <= endLevel &&
                result.Result.NetDamage <= 0);
        }

        private static int FailCount(IReadOnlyList<BalanceSimulationResult> results, int startLevel, int endLevel)
        {
            return results.Count(result =>
                result.Level.LevelId >= startLevel &&
                result.Level.LevelId <= endLevel &&
                result.Result.NetDamage > 0);
        }

        private static PrototypeBoardEvaluationResult GetResult(
            IReadOnlyList<BalanceSimulationResult> results,
            int levelId)
        {
            return results.Single(result => result.Level.LevelId == levelId).Result;
        }

        private static BoardModel CreateEmptyBoard()
        {
            return new BoardModel();
        }

        private static BoardModel CreateWeakBoard()
        {
            var board = new BoardModel();
            board.SetTile(new BoardPosition(0, 0), new TileData(TileType.Wood, 1));
            board.SetTile(new BoardPosition(1, 0), new TileData(TileType.Metal, 1));
            board.SetTile(new BoardPosition(2, 0), new TileData(TileType.Food, 1));
            board.SetTile(new BoardPosition(3, 0), new TileData(TileType.Energy, 1));
            return board;
        }

        private static BoardModel CreateMediumBoard()
        {
            var board = new BoardModel();
            board.SetTile(new BoardPosition(0, 0), new TileData(TileType.Wood, 2));
            board.SetTile(new BoardPosition(1, 0), new TileData(TileType.Wood, 2));
            board.SetTile(new BoardPosition(2, 0), new TileData(TileType.Metal, 2));
            board.SetTile(new BoardPosition(3, 0), new TileData(TileType.Metal, 2));
            board.SetTile(new BoardPosition(4, 0), new TileData(TileType.Food, 2));
            board.SetTile(new BoardPosition(5, 0), new TileData(TileType.Energy, 2));
            return board;
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

        private static BoardModel CreateLateStrongBoard()
        {
            var board = new BoardModel();
            board.SetTile(new BoardPosition(0, 0), new TileData(TileType.Wood, 4));
            board.SetTile(new BoardPosition(1, 0), new TileData(TileType.Metal, 4));
            board.SetTile(new BoardPosition(2, 0), new TileData(TileType.Food, 4));
            board.SetTile(new BoardPosition(3, 0), new TileData(TileType.Energy, 4));
            return board;
        }

        private readonly struct BalanceSimulationResult
        {
            public LevelDefinition Level { get; }
            public PrototypeBoardEvaluationResult Result { get; }

            public BalanceSimulationResult(LevelDefinition level, PrototypeBoardEvaluationResult result)
            {
                Level = level;
                Result = result;
            }
        }

        private readonly struct BoardAssumption
        {
            public string Name { get; }
            public Func<BoardModel> CreateBoard { get; }

            public BoardAssumption(string name, Func<BoardModel> createBoard)
            {
                Name = name;
                CreateBoard = createBoard;
            }
        }
    }
}
