using System;
using System.Collections.Generic;
using MergeShelter.Board;
using MergeShelter.Combat;

namespace MergeShelter.Levels
{
    [Serializable]
    public sealed class LevelDefinition
    {
        public int LevelId;
        public string DisplayName;
        public string Objective;
        public string TutorialMessage;
        public List<TileType> AvailableTiles = new();
        public List<EnemyData> Enemies = new();
        public int CoinReward;
        public int PartsReward;
    }
}
