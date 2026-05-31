using System;
using System.Collections.Generic;
using System.Linq;
using MergeShelter.Board;
using MergeShelter.Combat;
using MergeShelter.Economy;
using MergeShelter.Levels;
using MergeShelter.Meta;
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
                Assert.IsFalse(string.IsNullOrWhiteSpace(level.Objective), $"Level {level.LevelId} needs an objective.");
                Assert.LessOrEqual(level.Objective.Length, 60, $"Level {level.LevelId} objective should be concise for mobile.");
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
        public void Economy_FirstUpgradeIsAvailableAroundLevelsTwoOrThree()
        {
            var progression = new SessionProgressionState();
            var firstUpgradeCost = progression.ShelterUpgradeCost;

            Assert.AreEqual(100, firstUpgradeCost);
            Assert.Less(CumulativeCoinsThroughLevel(1), firstUpgradeCost);
            Assert.GreaterOrEqual(CumulativeCoinsThroughLevel(2), firstUpgradeCost);
            Assert.GreaterOrEqual(CumulativeCoinsThroughLevel(3), firstUpgradeCost);
        }

        [Test]
        public void Economy_SecondUpgradeFallsInTargetWindowWithNormalOrSupportPlay()
        {
            var normalUpgradeLevels = SimulateGreedyUpgradeLevels(includeDailyAndQuestSupport: false);
            var supportedUpgradeLevels = SimulateGreedyUpgradeLevels(includeDailyAndQuestSupport: true);

            Assert.GreaterOrEqual(normalUpgradeLevels.Count, 2);
            Assert.GreaterOrEqual(supportedUpgradeLevels.Count, 2);

            var normalSecondUpgradeLevel = normalUpgradeLevels[1];
            var supportedSecondUpgradeLevel = supportedUpgradeLevels[1];

            Assert.GreaterOrEqual(normalSecondUpgradeLevel, 5);
            Assert.LessOrEqual(normalSecondUpgradeLevel, 8);
            Assert.GreaterOrEqual(supportedSecondUpgradeLevel, 5);
            Assert.LessOrEqual(supportedSecondUpgradeLevel, 8);
            Assert.LessOrEqual(supportedSecondUpgradeLevel, normalSecondUpgradeLevel);
        }

        [Test]
        public void Economy_UpgradeCostProgressionIncreasesOverTime()
        {
            var progression = new SessionProgressionState();
            var costs = new List<int>();

            for (var i = 0; i < 6; i++)
            {
                var cost = progression.ShelterUpgradeCost;
                costs.Add(cost);
                progression.AddCurrency(CurrencyType.Coins, cost);
                Assert.IsTrue(progression.TryUpgradeShelter());
            }

            CollectionAssert.AreEqual(new[] { 100, 450, 1050 }, costs.Take(3).ToArray());
            for (var i = 1; i < costs.Count; i++)
                Assert.Greater(costs[i], costs[i - 1], $"Upgrade cost {i + 1} should be greater than upgrade cost {i}.");
        }

        [Test]
        public void Economy_RewardsDoNotMakeEveryLevelImmediatelyUpgradeAffordable()
        {
            var upgradeLevels = SimulateGreedyUpgradeLevels(includeDailyAndQuestSupport: false);

            Assert.Less(upgradeLevels.Count, 10, "Normal level rewards should not fund excessive upgrades by Level 30.");
            for (var i = 1; i < upgradeLevels.Count; i++)
                Assert.Greater(upgradeLevels[i] - upgradeLevels[i - 1], 1, "Normal rewards should not support upgrades on consecutive levels.");
        }

        [Test]
        public void Economy_DailyAndQuestSupportHelpsTimingWithoutTrivializingLateCurve()
        {
            var normalUpgradeLevels = SimulateGreedyUpgradeLevels(includeDailyAndQuestSupport: false);
            var supportedUpgradeLevels = SimulateGreedyUpgradeLevels(includeDailyAndQuestSupport: true);
            var supportCoins = DailyReward.DefaultCoinReward + DefaultQuestSupportCoins();
            var lateAverageReward = Average(SprintOneLevelCatalog.CreateLevels(), 21, 30, level => level.CoinReward);

            Assert.Less(supportedUpgradeLevels[1], normalUpgradeLevels[1]);
            Assert.LessOrEqual(supportedUpgradeLevels.Count, normalUpgradeLevels.Count + 1);
            Assert.Less(supportedUpgradeLevels.Count, 10, "Daily and quest support should not make the full 30-level curve upgrade-saturated.");
            Assert.Less(supportCoins, lateAverageReward, "One daily/quest support package should stay smaller than an average late-level reward.");
        }

        [Test]
        public void Economy_PartsIncreaseAcrossBandsButRemainConservative()
        {
            var levels = SprintOneLevelCatalog.CreateLevels();
            var lateAverageParts = Average(levels, 21, 30, level => level.PartsReward);
            var totalParts = levels.Sum(level => level.PartsReward);

            AssertBandAverageIncreases(levels, 1, 5, 6, 10, level => level.PartsReward, "parts rewards");
            AssertBandAverageIncreases(levels, 6, 10, 11, 20, level => level.PartsReward, "parts rewards");
            AssertBandAverageIncreases(levels, 11, 20, 21, 30, level => level.PartsReward, "parts rewards");
            Assert.LessOrEqual(lateAverageParts, 12d, "Parts should remain a secondary signal in the late prototype band.");
            Assert.LessOrEqual(totalParts, 225, "Total parts across Levels 1-30 should preserve future economy headroom.");
        }

        [Test]
        public void LevelOne_RemainsFirstRunTutorialEntry()
        {
            var level = SprintOneLevelCatalog.CreateLevels()[0];

            Assert.AreEqual(1, level.LevelId);
            Assert.AreEqual("First Night", level.DisplayName);
            Assert.IsFalse(string.IsNullOrWhiteSpace(level.Objective));
            Assert.That(level.Objective, Does.Contain("Survive"));
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

        [Test]
        public void AllEnemies_HaveDisplayNameAndBehaviorTag()
        {
            var levels = SprintOneLevelCatalog.CreateLevels();
            var seenIds = new HashSet<string>();

            foreach (var level in levels)
            {
                foreach (var enemy in level.Enemies)
                {
                    Assert.IsFalse(string.IsNullOrWhiteSpace(enemy.DisplayName), $"Enemy '{enemy.EnemyId}' in Level {level.LevelId} needs a display name.");
                    Assert.IsFalse(string.IsNullOrWhiteSpace(enemy.BehaviorTag), $"Enemy '{enemy.EnemyId}' in Level {level.LevelId} needs a behavior tag.");
                    Assert.LessOrEqual(enemy.DisplayName.Length, 20, $"Enemy '{enemy.EnemyId}' display name should be concise.");
                    Assert.LessOrEqual(enemy.BehaviorTag.Length, 10, $"Enemy '{enemy.EnemyId}' behavior tag should be short.");
                    Assert.That(enemy.DisplayName, Does.Not.Contain("_"), $"Display name should not expose raw ID underscore for '{enemy.EnemyId}'.");
                    seenIds.Add(enemy.EnemyId);
                }
            }

            Assert.GreaterOrEqual(seenIds.Count, 11, "All 11 enemy types should appear across the 30 levels.");
        }

        [Test]
        public void WaveRoster_GroupsDuplicateEnemies()
        {
            var enemies = new List<EnemyData>
            {
                new EnemyData { EnemyId = "walker", DisplayName = "Walker", BehaviorTag = "basic" },
                new EnemyData { EnemyId = "walker", DisplayName = "Walker", BehaviorTag = "basic" },
                new EnemyData { EnemyId = "bomber", DisplayName = "Bomber", BehaviorTag = "walls" }
            };

            var roster = EnemyData.FormatWaveRoster(enemies);
            Assert.That(roster, Does.StartWith("Wave:"));
            Assert.That(roster, Does.Contain("2× Walker"));
            Assert.That(roster, Does.Contain("Bomber"));
            Assert.That(roster, Does.Not.Contain("walker"));
            Assert.That(roster, Does.Not.Contain("bomber"));
        }

        [Test]
        public void WaveRoster_LevelOneIsReadable()
        {
            var level = SprintOneLevelCatalog.CreateLevels()[0];
            var roster = EnemyData.FormatWaveRoster(level.Enemies);

            Assert.That(roster, Does.StartWith("Wave:"));
            Assert.That(roster, Does.Contain("Walker"));
            Assert.That(roster, Does.Contain("basic"));
            Assert.LessOrEqual(roster.Length, 80, "Level 1 roster should be short.");
        }

        [Test]
        public void WaveRoster_LevelThirtyIsReadable()
        {
            var level = SprintOneLevelCatalog.CreateLevels()[29];
            var roster = EnemyData.FormatWaveRoster(level.Enemies);

            Assert.That(roster, Does.StartWith("Wave:"));
            Assert.That(roster, Does.Not.Contain("_"));
            Assert.Greater(roster.Length, 20, "Level 30 roster should list multiple enemy types.");
        }

        [Test]
        public void DefeatHints_AreActionableAndDoNotExposeRawFailReasons()
        {
            var failReasons = new[]
            {
                PrototypeBoardEvaluator.WeakWall,
                PrototypeBoardEvaluator.LowAttack,
                PrototypeBoardEvaluator.NoHeal,
                PrototypeBoardEvaluator.NoEnergy,
                PrototypeBoardEvaluator.BoardBlocked,
                PrototypeBoardEvaluator.Overwhelmed,
                PrototypeBoardEvaluator.Unknown
            };

            foreach (var reason in failReasons)
            {
                var hint = PrototypeBoardEvaluator.GetDefeatHint(reason);
                Assert.IsFalse(string.IsNullOrWhiteSpace(hint), $"Fail reason '{reason}' should produce a hint.");
                Assert.That(hint, Does.Not.Contain("weak_wall"));
                Assert.That(hint, Does.Not.Contain("low_attack"));
                Assert.That(hint, Does.Not.Contain("no_heal"));
                Assert.That(hint, Does.Not.Contain("no_energy"));
                Assert.That(hint, Does.Not.Contain("board_blocked"));
                Assert.That(hint, Does.Not.Contain("overwhelmed"));
                Assert.That(hint, Does.Not.Contain("unknown"));
                Assert.LessOrEqual(hint.Length, 60, $"Hint for '{reason}' should be concise.");
            }

            Assert.IsFalse(string.IsNullOrWhiteSpace(PrototypeBoardEvaluator.GetDefeatHint(null)));
            Assert.IsFalse(string.IsNullOrWhiteSpace(PrototypeBoardEvaluator.GetDefeatHint(string.Empty)));
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

        private static int CumulativeCoinsThroughLevel(int endLevel)
        {
            return SprintOneLevelCatalog.CreateLevels()
                .Where(level => level.LevelId <= endLevel)
                .Sum(level => level.CoinReward);
        }

        private static int DefaultQuestSupportCoins()
        {
            return new DailyQuestModel()
                .GetQuestStates()
                .Sum(quest => quest.RewardCoins);
        }

        private static IReadOnlyList<int> SimulateGreedyUpgradeLevels(bool includeDailyAndQuestSupport)
        {
            var progression = new SessionProgressionState();
            var upgradeLevels = new List<int>();
            var supportApplied = false;

            foreach (var level in SprintOneLevelCatalog.CreateLevels())
            {
                progression.AddCurrency(CurrencyType.Coins, level.CoinReward);
                if (includeDailyAndQuestSupport && !supportApplied && level.LevelId == 1)
                {
                    progression.AddCurrency(CurrencyType.Coins, DailyReward.DefaultCoinReward + DefaultQuestSupportCoins());
                    supportApplied = true;
                }

                while (progression.CanAffordShelterUpgrade)
                {
                    Assert.IsTrue(progression.TryUpgradeShelter());
                    upgradeLevels.Add(level.LevelId);
                }
            }

            return upgradeLevels;
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
