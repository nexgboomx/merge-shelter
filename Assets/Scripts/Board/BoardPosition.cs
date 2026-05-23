using System;

namespace MergeShelter.Board
{
    [Serializable]
    public struct BoardPosition
    {
        public int X;
        public int Y;

        public BoardPosition(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
