using System.Collections.Generic;
using System.Linq;

namespace MergeShelter.Board
{
    public sealed class MergeResolver
    {
        public bool TryResolveMerge(BoardModel board, BoardPosition origin, out TileData mergedTile)
        {
            mergedTile = TileData.Empty;

            if (!board.IsInside(origin))
                return false;

            var tile = board.GetTile(origin);
            if (tile.IsEmpty)
                return false;

            var connected = FindConnectedSameTiles(board, origin, tile);
            if (connected.Count < 3)
                return false;

            var consumed = connected.Take(3).ToList();
            foreach (var pos in consumed)
                board.ClearTile(pos);

            mergedTile = new TileData(tile.Type, tile.Tier + 1);
            board.SetTile(origin, mergedTile);
            return true;
        }

        private List<BoardPosition> FindConnectedSameTiles(BoardModel board, BoardPosition start, TileData target)
        {
            var result = new List<BoardPosition>();
            var visited = new bool[board.Width, board.Height];
            var queue = new Queue<BoardPosition>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (!board.IsInside(current) || visited[current.X, current.Y])
                    continue;

                visited[current.X, current.Y] = true;
                var tile = board.GetTile(current);
                if (tile.Type != target.Type || tile.Tier != target.Tier)
                    continue;

                result.Add(current);

                queue.Enqueue(new BoardPosition(current.X + 1, current.Y));
                queue.Enqueue(new BoardPosition(current.X - 1, current.Y));
                queue.Enqueue(new BoardPosition(current.X, current.Y + 1));
                queue.Enqueue(new BoardPosition(current.X, current.Y - 1));
            }

            return result;
        }
    }
}
