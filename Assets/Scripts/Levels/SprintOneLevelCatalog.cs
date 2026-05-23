using System.Collections.Generic;
using MergeShelter.Board;
using MergeShelter.Combat;

namespace MergeShelter.Levels
{
    public static class SprintOneLevelCatalog
    {
        public static IReadOnlyList<LevelDefinition> CreateLevels()
        {
            return new List<LevelDefinition>
            {
                CreateLevel(1, "First Night", "Drag 3 Wood tiles together to build a stronger wall.",
                    new[] { TileType.Wood }, 50,
                    Walker(), Walker()),

                CreateLevel(2, "Scrap Defense", "Merge Metal to boost your turret power.",
                    new[] { TileType.Wood, TileType.Metal }, 70,
                    Walker(), Walker(), Walker()),

                CreateLevel(3, "Hold the Gate", "Walls buy time. Turrets end the wave. Use both.",
                    new[] { TileType.Wood, TileType.Metal }, 80,
                    Walker(), Walker(), Walker(), Walker()),

                CreateLevel(4, "Fast Shadows", "Runners hit fast. Merge early before the wave arrives.",
                    new[] { TileType.Wood, TileType.Metal }, 100,
                    Walker(), Walker(), Runner(), Runner()),

                CreateLevel(5, "Emergency Meal", "Merge Food to recover damaged shelter HP.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food }, 120,
                    Walker(), Walker(), Walker(), Runner(), Runner()),

                CreateLevel(6, "Power Surge", "Merge Energy to charge an emergency skill.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy }, 130,
                    Walker(), Walker(), Walker(), Walker(), Runner(), Runner()),

                CreateLevel(7, "Heavy Footsteps", "Tanks are slow but tough. Prepare stronger Metal merges.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Energy }, 150,
                    Tank(), Walker(), Walker(), Walker()),

                CreateLevel(8, "Broken Line", "Bad placement can block future merges. Keep space open.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food }, 170,
                    Tank(), Tank(), Runner(), Runner()),

                CreateLevel(9, "Fuse Warning", "Bombers punish weak walls. Upgrade Wood before impact.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Energy }, 180,
                    Bomber(), Runner(), Runner(), Walker(), Walker()),

                CreateLevel(10, "Night Boss", "Survive by combining defense, attack, heal, and energy.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy }, 250,
                    Tank(), Bomber(), Walker(), Walker(), Walker(), Walker())
            };
        }

        private static LevelDefinition CreateLevel(
            int id,
            string name,
            string tutorial,
            IEnumerable<TileType> availableTiles,
            int coinReward,
            params EnemyData[] enemies)
        {
            return new LevelDefinition
            {
                LevelId = id,
                DisplayName = name,
                TutorialMessage = tutorial,
                AvailableTiles = new List<TileType>(availableTiles),
                Enemies = new List<EnemyData>(enemies),
                CoinReward = coinReward,
                PartsReward = id >= 10 ? 5 : id >= 7 ? 2 : id >= 4 ? 1 : 0
            };
        }

        private static EnemyData Walker() => new EnemyData
        {
            EnemyId = "walker",
            MaxHealth = 10,
            Damage = 8,
            Speed = 1.0f
        };

        private static EnemyData Runner() => new EnemyData
        {
            EnemyId = "runner",
            MaxHealth = 8,
            Damage = 6,
            Speed = 1.6f
        };

        private static EnemyData Tank() => new EnemyData
        {
            EnemyId = "tank",
            MaxHealth = 25,
            Damage = 14,
            Speed = 0.6f
        };

        private static EnemyData Bomber() => new EnemyData
        {
            EnemyId = "bomber",
            MaxHealth = 15,
            Damage = 22,
            Speed = 0.8f
        };
    }
}
