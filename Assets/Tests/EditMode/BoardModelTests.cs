using MergeShelter.Board;
using NUnit.Framework;

namespace MergeShelter.Tests.EditMode
{
    public sealed class BoardModelTests
    {
        [Test]
        public void DefaultBoard_IsSixBySix()
        {
            var board = new BoardModel();

            Assert.AreEqual(6, board.Width);
            Assert.AreEqual(6, board.Height);
        }

        [Test]
        public void TryPlace_StoresTileAtCoordinate()
        {
            var board = new BoardModel();
            var position = new BoardPosition(2, 3);
            var tile = new TileData(TileType.Metal, 1);

            var placed = board.TryPlace(position, tile);
            var storedTile = board.GetTile(position);

            Assert.IsTrue(placed);
            Assert.AreEqual(TileType.Metal, storedTile.Type);
            Assert.AreEqual(1, storedTile.Tier);
        }
    }
}
