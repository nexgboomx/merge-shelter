using System;

namespace MergeShelter.Board
{
    [Serializable]
    public struct TileData
    {
        public TileType Type;
        public int Tier;

        public bool IsEmpty => Type == TileType.None || Tier <= 0;

        public TileData(TileType type, int tier)
        {
            Type = type;
            Tier = tier;
        }

        public static TileData Empty => new TileData(TileType.None, 0);
    }
}
