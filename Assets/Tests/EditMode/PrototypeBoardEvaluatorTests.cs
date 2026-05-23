using System.Collections.Generic;
using MergeShelter.Board;
using MergeShelter.Combat;
using NUnit.Framework;

namespace MergeShelter.Tests.EditMode
{
    public sealed class PrototypeBoardEvaluatorTests
    {
        [Test]
        public void EmptyBoard_HasWeakCombatValue()
        {
            var result = Evaluate(new BoardModel());

            Assert.AreEqual(0, result.TotalProtection);
            Assert.Greater(result.EnemyPressure, 0);
            Assert.AreEqual(result.EnemyPressure, result.NetDamage);
            Assert.AreEqual(PrototypeBoardEvaluator.WeakWall, result.FailReason);
        }

        [Test]
        public void Wood_IncreasesDefense()
        {
            var board = new BoardModel();
            board.TryPlace(new BoardPosition(0, 0), new TileData(TileType.Wood, 1));

            var result = Evaluate(board);

            Assert.Greater(result.WoodDefense, 0);
            Assert.AreEqual(result.WoodDefense, result.TotalProtection);
        }

        [Test]
        public void Metal_IncreasesAttack()
        {
            var board = new BoardModel();
            board.TryPlace(new BoardPosition(0, 0), new TileData(TileType.Metal, 1));

            var result = Evaluate(board);

            Assert.Greater(result.MetalAttack, 0);
            Assert.AreEqual(result.MetalAttack, result.TotalProtection);
        }

        [Test]
        public void Food_IncreasesHealing()
        {
            var board = new BoardModel();
            board.TryPlace(new BoardPosition(0, 0), new TileData(TileType.Food, 1));

            var result = Evaluate(board);

            Assert.Greater(result.FoodHealing, 0);
            Assert.AreEqual(result.FoodHealing, result.TotalProtection);
        }

        [Test]
        public void Energy_IncreasesShield()
        {
            var board = new BoardModel();
            board.TryPlace(new BoardPosition(0, 0), new TileData(TileType.Energy, 1));

            var result = Evaluate(board);

            Assert.Greater(result.EnergyShield, 0);
            Assert.AreEqual(result.EnergyShield, result.TotalProtection);
        }

        [Test]
        public void HigherTierTiles_ProduceStrongerValuesThanTierOne()
        {
            var tierOneBoard = new BoardModel();
            tierOneBoard.TryPlace(new BoardPosition(0, 0), new TileData(TileType.Wood, 1));

            var tierTwoBoard = new BoardModel();
            tierTwoBoard.TryPlace(new BoardPosition(0, 0), new TileData(TileType.Wood, 2));

            var tierOneResult = Evaluate(tierOneBoard);
            var tierTwoResult = Evaluate(tierTwoBoard);

            Assert.Greater(tierTwoResult.WoodDefense, tierOneResult.WoodDefense);
        }

        [Test]
        public void Evaluator_ReturnsUsefulFailReason()
        {
            var result = Evaluate(new BoardModel());

            Assert.Contains(result.FailReason, new[]
            {
                PrototypeBoardEvaluator.WeakWall,
                PrototypeBoardEvaluator.LowAttack,
                PrototypeBoardEvaluator.NoHeal,
                PrototypeBoardEvaluator.NoEnergy,
                PrototypeBoardEvaluator.BoardBlocked,
                PrototypeBoardEvaluator.Overwhelmed,
                PrototypeBoardEvaluator.Unknown
            });
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.ResultExplanation));
        }

        private static PrototypeBoardEvaluationResult Evaluate(BoardModel board)
        {
            return new PrototypeBoardEvaluator().Evaluate(board, CreateHardWave());
        }

        private static IReadOnlyList<EnemyData> CreateHardWave()
        {
            return new List<EnemyData>
            {
                new EnemyData { EnemyId = "tank", MaxHealth = 25, Damage = 14, Speed = 0.6f },
                new EnemyData { EnemyId = "bomber", MaxHealth = 15, Damage = 22, Speed = 0.8f },
                new EnemyData { EnemyId = "walker", MaxHealth = 10, Damage = 8, Speed = 1.0f },
                new EnemyData { EnemyId = "walker", MaxHealth = 10, Damage = 8, Speed = 1.0f },
                new EnemyData { EnemyId = "walker", MaxHealth = 10, Damage = 8, Speed = 1.0f },
                new EnemyData { EnemyId = "walker", MaxHealth = 10, Damage = 8, Speed = 1.0f }
            };
        }
    }
}
