using System;
using System.Collections.Generic;
using MergeShelter.Board;

namespace MergeShelter.Combat
{
    public sealed class PrototypeBoardEvaluationResult
    {
        public int WoodDefense { get; set; }
        public int MetalAttack { get; set; }
        public int FoodHealing { get; set; }
        public int EnergyShield { get; set; }
        public int TotalProtection { get; set; }
        public int EnemyPressure { get; set; }
        public int NetDamage { get; set; }
        public string FailReason { get; set; }
        public string ResultExplanation { get; set; }
    }

    public sealed class PrototypeBoardEvaluator
    {
        public const string WeakWall = "weak_wall";
        public const string LowAttack = "low_attack";
        public const string NoHeal = "no_heal";
        public const string NoEnergy = "no_energy";
        public const string BoardBlocked = "board_blocked";
        public const string Overwhelmed = "overwhelmed";
        public const string Unknown = "unknown";

        private const int WoodDefenseMultiplier = 8;
        private const int MetalAttackMultiplier = 6;
        private const int FoodHealingMultiplier = 5;
        private const int EnergyShieldMultiplier = 7;

        public PrototypeBoardEvaluationResult Evaluate(BoardModel board, IReadOnlyList<EnemyData> enemies)
        {
            if (board == null)
                throw new ArgumentNullException(nameof(board));

            var result = new PrototypeBoardEvaluationResult();
            var occupiedCells = 0;

            for (var x = 0; x < board.Width; x++)
            {
                for (var y = 0; y < board.Height; y++)
                {
                    var tile = board.GetTile(new BoardPosition(x, y));
                    if (tile.IsEmpty)
                        continue;

                    occupiedCells++;
                    var contribution = tile.Tier * tile.Tier;

                    switch (tile.Type)
                    {
                        case TileType.Wood:
                            result.WoodDefense += contribution * WoodDefenseMultiplier;
                            break;
                        case TileType.Metal:
                            result.MetalAttack += contribution * MetalAttackMultiplier;
                            break;
                        case TileType.Food:
                            result.FoodHealing += contribution * FoodHealingMultiplier;
                            break;
                        case TileType.Energy:
                            result.EnergyShield += contribution * EnergyShieldMultiplier;
                            break;
                    }
                }
            }

            result.EnemyPressure = CalculateEnemyPressure(enemies);
            result.TotalProtection = result.WoodDefense + result.MetalAttack + result.FoodHealing + result.EnergyShield;
            result.NetDamage = Math.Max(0, result.EnemyPressure - result.TotalProtection);
            result.FailReason = DetermineFailReason(result, occupiedCells, board.Width * board.Height);
            result.ResultExplanation = BuildExplanation(result);
            return result;
        }

        public static string GetDefeatHint(string failReason)
        {
            switch (failReason)
            {
                case WeakWall:
                    return "Try merging more Wood tiles for stronger walls.";
                case LowAttack:
                    return "Merge Metal tiles to boost your attack power.";
                case NoHeal:
                    return "Add Food tiles to recover shelter HP.";
                case NoEnergy:
                    return "Merge Energy tiles to charge your shields.";
                case BoardBlocked:
                    return "Leave empty spaces for future merges.";
                case Overwhelmed:
                    return "Upgrade your shelter and build higher-tier merges.";
                default:
                    return "Build a balanced board before starting the wave.";
            }
        }

        private static int CalculateEnemyPressure(IReadOnlyList<EnemyData> enemies)
        {
            if (enemies == null || enemies.Count == 0)
                return 0;

            var pressure = 0;
            foreach (var enemy in enemies)
            {
                if (enemy == null)
                    continue;

                pressure += Math.Max(0, enemy.Damage);
                pressure += (Math.Max(0, enemy.MaxHealth) + 1) / 2;
            }

            return pressure;
        }

        private static string DetermineFailReason(PrototypeBoardEvaluationResult result, int occupiedCells, int totalCells)
        {
            if (result.NetDamage <= 0)
                return string.Empty;

            if (occupiedCells >= totalCells && result.TotalProtection < result.EnemyPressure)
                return BoardBlocked;

            if (result.WoodDefense < Math.Max(16, result.EnemyPressure / 4))
                return WeakWall;

            if (result.MetalAttack < Math.Max(12, result.EnemyPressure / 5))
                return LowAttack;

            if (result.EnemyPressure >= 70 && result.FoodHealing <= 0)
                return NoHeal;

            if (result.EnemyPressure >= 90 && result.EnergyShield <= 0)
                return NoEnergy;

            return Overwhelmed;
        }

        private static string BuildExplanation(PrototypeBoardEvaluationResult result)
        {
            if (result.NetDamage <= 0)
                return BuildVictoryExplanation(result);

            switch (result.FailReason)
            {
                case WeakWall:
                    return "Defeat. You needed more Wood defense before starting the wave.";
                case LowAttack:
                    return "Defeat. Your attack power was too low.";
                case NoHeal:
                    return "Defeat. You had no recovery. Merge Food before hard waves.";
                case NoEnergy:
                    return "Defeat. Emergency power was not charged. Merge Energy earlier.";
                case BoardBlocked:
                    return "Defeat. Your board was blocked before you built enough power.";
                case Overwhelmed:
                    return "Defeat. Your shelter was overwhelmed. Build stronger merges before the wave.";
                default:
                    return "Defeat. Your shelter failed for an unknown reason.";
            }
        }

        private static string BuildVictoryExplanation(PrototypeBoardEvaluationResult result)
        {
            var strongestValue = result.WoodDefense;
            var explanation = "Victory! Your upgraded walls absorbed the attack.";

            if (result.MetalAttack > strongestValue)
            {
                strongestValue = result.MetalAttack;
                explanation = "Victory! Metal turrets reduced the enemy pressure.";
            }

            if (result.FoodHealing > strongestValue)
            {
                strongestValue = result.FoodHealing;
                explanation = "Victory! Food supplies restored the shelter through the attack.";
            }

            if (result.EnergyShield > strongestValue)
                explanation = "Victory! Energy shielded the shelter at the critical moment.";

            return explanation;
        }
    }
}
