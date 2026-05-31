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
                CreateLevel(1, "First Night", "Survive the first wave",
                    "Drag 3 Wood tiles together to build a stronger wall.",
                    new[] { TileType.Wood }, 50, 0,
                    Walker(), Walker()),

                CreateLevel(2, "Scrap Defense", "Survive with walls and turrets",
                    "Merge Metal to boost your turret power.",
                    new[] { TileType.Wood, TileType.Metal }, 70, 0,
                    Walker(), Walker(), Walker()),

                CreateLevel(3, "Hold the Gate", "Defend against a larger group",
                    "Walls buy time. Turrets end the wave. Use both.",
                    new[] { TileType.Wood, TileType.Metal }, 80, 0,
                    Walker(), Walker(), Walker(), Walker()),

                CreateLevel(4, "Fast Shadows", "Stop the runners before they reach the shelter",
                    "Runners hit fast. Merge early before the wave arrives.",
                    new[] { TileType.Wood, TileType.Metal }, 100, 1,
                    Walker(), Walker(), Runner(), Runner()),

                CreateLevel(5, "Emergency Meal", "Survive using Food to heal",
                    "Merge Food to recover damaged shelter HP.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food }, 120, 1,
                    Walker(), Walker(), Walker(), Runner(), Runner()),

                CreateLevel(6, "Power Surge", "Survive using all four tile types",
                    "Merge Energy to charge an emergency skill.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy }, 130, 1,
                    Walker(), Walker(), Walker(), Walker(), Runner(), Runner()),

                CreateLevel(7, "Heavy Footsteps", "Defeat a tank with strong merges",
                    "Tanks are slow but tough. Prepare stronger Metal merges.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Energy }, 150, 2,
                    Tank(), Walker(), Walker(), Walker()),

                CreateLevel(8, "Broken Line", "Survive without blocking your own board",
                    "Bad placement can block future merges. Keep space open.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food }, 170, 2,
                    Tank(), Tank(), Runner(), Runner()),

                CreateLevel(9, "Fuse Warning", "Protect your walls from bombers",
                    "Bombers punish weak walls. Upgrade Wood before impact.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Energy }, 180, 2,
                    Bomber(), Runner(), Runner(), Walker(), Walker()),

                CreateLevel(10, "Night Boss", "Survive the first boss wave",
                    "Survive by combining defense, attack, heal, and energy.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy }, 250, 5,
                    Tank(), Bomber(), Walker(), Walker(), Walker(), Walker()),

                CreateLevel(11, "Barricade Drill", "Hold with Wood and Metal against a mixed assault",
                    "Use Wood and Metal merges before claiming harder rewards.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food }, 280, 5,
                    Tank(), Bomber(), Runner(), Walker(), Walker(), Walker()),

                CreateLevel(12, "Runner Pack", "Fill the board before fast enemies arrive",
                    "Runners punish empty boards. Fill lanes before the wave.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food }, 310, 5,
                    Tank(), Tank(), Bomber(), Walker(), Walker()),

                CreateLevel(13, "Supply Rush", "Keep HP safe with Food while Metal clears threats",
                    "Food keeps HP above danger while Metal clears stragglers.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy }, 340, 5,
                    Tank(), Bomber(), Bomber(), Runner(), Runner(), Walker()),

                CreateLevel(14, "Power Cache", "Use Energy shields against a mixed wave",
                    "Energy shields start to matter against mixed waves.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy }, 370, 6,
                    Bruiser(), Tank(), Bomber(), Walker(), Walker()),

                CreateLevel(15, "Double Barricade", "Build two strong merge groups before the wave",
                    "Build two useful merge groups before starting the wave.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food }, 400, 6,
                    Bruiser(), Tank(), Tank(), Runner(), Runner(), Walker()),

                CreateLevel(16, "Signal Fire", "Balance attack and recovery before the assault",
                    "Balance attack and shelter recovery before the pressure lands.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy }, 430, 6,
                    Bruiser(), Tank(), Bomber(), Bomber(), Walker()),

                CreateLevel(17, "Scrap Convoy", "Survive with upgraded merges against heavy pressure",
                    "Upgrades and cleaner merges should carry this convoy fight.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Energy }, 470, 7,
                    Bruiser(), Bruiser(), Tank(), Bomber(), Runner()),

                CreateLevel(18, "Flooded Street", "Preserve HP through sustained enemy pressure",
                    "Food and Energy help preserve HP through longer pressure.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy }, 510, 7,
                    SiegeTank(), Bruiser(), Tank(), Runner(), Runner(), Walker()),

                CreateLevel(19, "Reinforced Wave", "Survive a reinforced wave with shelter upgrades",
                    "A medium board can win if upgrades are not skipped.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy }, 560, 8,
                    SiegeTank(), Bruiser(), Bruiser(), Bomber()),

                CreateLevel(20, "Second Night Boss", "Survive the second boss using all resources",
                    "Use every tile type and shelter upgrades to stabilize.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy }, 620, 8,
                    SiegeTank(), Demolisher(), Bruiser(), Tank(), Bomber()),

                CreateLevel(21, "Outer District", "Plan your board before placing any tiles",
                    "Late waves expect planned merges before the first tap on Start.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy }, 680, 9,
                    SiegeTank(), Demolisher(), Bruiser(), Bruiser(), Runner()),

                CreateLevel(22, "Noisy Breach", "Hold the line with shelter upgrades and strong tiles",
                    "Tanks test whether coins were spent on shelter upgrades.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy }, 740, 9,
                    SiegeTank(), Demolisher(), Bruiser(), Tank(), Tank()),

                CreateLevel(23, "Cracked Overpass", "Defeat heavy enemies with high-tier merges",
                    "Heavy enemies require stronger Wood and Metal tiers.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy }, 800, 10,
                    Bulwark(), SiegeTank(), Demolisher(), Bomber()),

                CreateLevel(24, "Blackout Line", "Shield against demolishers with Energy merges",
                    "Energy shields buy time when demolishers arrive together.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy }, 870, 10,
                    Bulwark(), SiegeTank(), Demolisher(), Demolisher(), Runner()),

                CreateLevel(25, "Supply Collapse", "Build a strong board or the shelter collapses",
                    "Strong boards should win, but weak boards will collapse.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy }, 940, 11,
                    Bulwark(), SiegeTank(), SiegeTank(), Demolisher(), Bomber()),

                CreateLevel(26, "Last Safe Block", "Spend all resources before entering this fight",
                    "Use rewards, quests, and upgrades before taking this fight.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy }, 1020, 11,
                    AlphaBomber(), Bulwark(), SiegeTank(), Demolisher(), Bruiser()),

                CreateLevel(27, "Pressure Lock", "Plan tile placement instead of filling randomly",
                    "Board planning matters more than filling random spaces.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy }, 1100, 12,
                    AlphaBomber(), Bulwark(), SiegeTank(), SiegeTank(), Demolisher()),

                CreateLevel(28, "Shelter Siege", "Barely survive with a strong mixed board",
                    "A strong mixed board should barely hold the line.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy }, 1190, 13,
                    Colossus(), AlphaBomber(), Bulwark(), SiegeTank(), StormRunner()),

                CreateLevel(29, "Final Perimeter", "Win with top-tier merges and well-timed upgrades",
                    "Top-tier merges and upgrade timing decide this wave.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy }, 1290, 14,
                    Colossus(), AlphaBomber(), AlphaBomber(), Bulwark(), StormRunner()),

                CreateLevel(30, "Prototype Stand", "Hold the final stand with your strongest board",
                    "Endpoint check: build a strong mixed board before starting.",
                    new[] { TileType.Wood, TileType.Metal, TileType.Food, TileType.Energy }, 1400, 15,
                    Colossus(), AlphaBomber(), Bulwark(), SiegeTank(), Demolisher(), StormRunner())
            };
        }

        private static LevelDefinition CreateLevel(
            int id,
            string name,
            string objective,
            string tutorial,
            IEnumerable<TileType> availableTiles,
            int coinReward,
            int partsReward,
            params EnemyData[] enemies)
        {
            return new LevelDefinition
            {
                LevelId = id,
                DisplayName = name,
                Objective = objective,
                TutorialMessage = tutorial,
                AvailableTiles = new List<TileType>(availableTiles),
                Enemies = new List<EnemyData>(enemies),
                CoinReward = coinReward,
                PartsReward = partsReward
            };
        }

        private static EnemyData Walker() => CreateEnemy("walker", "Walker", "basic", 10, 8, 1.0f);

        private static EnemyData Runner() => CreateEnemy("runner", "Runner", "fast", 8, 6, 1.6f);

        private static EnemyData Tank() => CreateEnemy("tank", "Tank", "tough", 25, 14, 0.6f);

        private static EnemyData Bomber() => CreateEnemy("bomber", "Bomber", "walls", 15, 22, 0.8f);

        private static EnemyData Bruiser() => CreateEnemy("bruiser", "Bruiser", "strong", 38, 18, 0.7f);

        private static EnemyData SiegeTank() => CreateEnemy("siege_tank", "Siege Tank", "heavy", 52, 24, 0.5f);

        private static EnemyData Demolisher() => CreateEnemy("demolisher", "Demolisher", "destroy", 32, 34, 0.75f);

        private static EnemyData Bulwark() => CreateEnemy("bulwark", "Bulwark", "armored", 70, 28, 0.45f);

        private static EnemyData StormRunner() => CreateEnemy("storm_runner", "Storm Runner", "rush", 26, 24, 1.7f);

        private static EnemyData AlphaBomber() => CreateEnemy("alpha_bomber", "Alpha Bomber", "blast", 45, 46, 0.7f);

        private static EnemyData Colossus() => CreateEnemy("colossus", "Colossus", "massive", 85, 42, 0.35f);

        private static EnemyData CreateEnemy(string id, string displayName, string behaviorTag, int maxHealth, int damage, float speed) => new EnemyData
        {
            EnemyId = id,
            DisplayName = displayName,
            BehaviorTag = behaviorTag,
            MaxHealth = maxHealth,
            Damage = damage,
            Speed = speed
        };
    }
}
