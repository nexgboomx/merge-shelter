using System;

namespace MergeShelter.Board
{
    public sealed class BoardModel
    {
        public const int DefaultWidth = 6;
        public const int DefaultHeight = 6;

        private readonly TileData[,] _tiles;

        public int Width { get; }
        public int Height { get; }

        public BoardModel(int width = DefaultWidth, int height = DefaultHeight)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("Board size must be positive.");

            Width = width;
            Height = height;
            _tiles = new TileData[width, height];

            Clear();
        }

        public bool IsInside(BoardPosition position)
        {
            return position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height;
        }

        public TileData GetTile(BoardPosition position)
        {
            EnsureInside(position);
            return _tiles[position.X, position.Y];
        }

        public bool CanPlace(BoardPosition position)
        {
            return IsInside(position) && _tiles[position.X, position.Y].IsEmpty;
        }

        public bool TryPlace(BoardPosition position, TileData tile)
        {
            if (!CanPlace(position) || tile.IsEmpty)
                return false;

            _tiles[position.X, position.Y] = tile;
            return true;
        }

        public void SetTile(BoardPosition position, TileData tile)
        {
            EnsureInside(position);
            _tiles[position.X, position.Y] = tile;
        }

        public void ClearTile(BoardPosition position)
        {
            EnsureInside(position);
            _tiles[position.X, position.Y] = TileData.Empty;
        }

        public void Clear()
        {
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                _tiles[x, y] = TileData.Empty;
        }

        private void EnsureInside(BoardPosition position)
        {
            if (!IsInside(position))
                throw new ArgumentOutOfRangeException(nameof(position), "Position outside board.");
        }
    }
}
