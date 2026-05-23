using MergeShelter.Board;
using NUnit.Framework;

namespace MergeShelter.Tests.EditMode
{
    public sealed class MergeResolverTests
    {
        [Test]
        public void ThreeConnectedSameTiles_MergeIntoNextTier()
        {
            var board = new BoardModel();
            var resolver = new MergeResolver();

            board.TryPlace(new BoardPosition(0, 0), new TileData(TileType.Wood, 1));
            board.TryPlace(new BoardPosition(1, 0), new TileData(TileType.Wood, 1));
            board.TryPlace(new BoardPosition(2, 0), new TileData(TileType.Wood, 1));

            var merged = resolver.TryResolveMerge(board, new BoardPosition(0, 0), out var mergedTile);

            Assert.IsTrue(merged);
            Assert.AreEqual(TileType.Wood, mergedTile.Type);
            Assert.AreEqual(2, mergedTile.Tier);
        }

        [Test]
        public void DifferentTiles_DoNotMerge()
        {
            var board = new BoardModel();
            var resolver = new MergeResolver();

            board.TryPlace(new BoardPosition(0, 0), new TileData(TileType.Wood, 1));
            board.TryPlace(new BoardPosition(1, 0), new TileData(TileType.Metal, 1));
            board.TryPlace(new BoardPosition(2, 0), new TileData(TileType.Wood, 1));

            var merged = resolver.TryResolveMerge(board, new BoardPosition(0, 0), out _);

            Assert.IsFalse(merged);
        }
    }
}
