using System;
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

        [Test]
        public void TryPlace_OccupiedCellRejectsPlacement()
        {
            var board = new BoardModel();
            var position = new BoardPosition(1, 1);

            var firstPlaced = board.TryPlace(position, new TileData(TileType.Wood, 1));
            var secondPlaced = board.TryPlace(position, new TileData(TileType.Metal, 1));
            var storedTile = board.GetTile(position);

            Assert.IsTrue(firstPlaced);
            Assert.IsFalse(secondPlaced);
            Assert.AreEqual(TileType.Wood, storedTile.Type);
        }

        [Test]
        public void GetTile_OutOfBoundsThrows()
        {
            var board = new BoardModel();

            Assert.Throws<ArgumentOutOfRangeException>(() => board.GetTile(new BoardPosition(-1, 0)));
            Assert.Throws<ArgumentOutOfRangeException>(() => board.GetTile(new BoardPosition(0, BoardModel.DefaultHeight)));
        }

        [Test]
        public void Clear_ResetsTiles()
        {
            var board = new BoardModel();
            var position = new BoardPosition(4, 4);
            board.TryPlace(position, new TileData(TileType.Energy, 2));

            board.Clear();

            Assert.IsTrue(board.GetTile(position).IsEmpty);
        }
    }
}
