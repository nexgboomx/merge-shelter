using System;
using System.Collections.Generic;
using MergeShelter.Levels;
using Random = UnityEngine.Random;

namespace MergeShelter.Board
{
    public sealed class PrototypeTileGenerator
    {
        private readonly List<TileType> _availableTiles = new();

        public void Configure(LevelDefinition level)
        {
            if (level == null)
                throw new ArgumentNullException(nameof(level));

            _availableTiles.Clear();
            _availableTiles.AddRange(level.AvailableTiles);

            if (_availableTiles.Count == 0)
                _availableTiles.Add(TileType.Wood);
        }

        public TileData GenerateNextTile()
        {
            var index = Random.Range(0, _availableTiles.Count);
            return new TileData(_availableTiles[index], 1);
        }
    }
}
