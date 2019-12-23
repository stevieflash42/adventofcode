using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Day20.Enums
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public static class DirectionHelper
    {
        public static Direction GetOppositeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    return Direction.Up;
                case Direction.Up:
                    return Direction.Down;
                case Direction.Right:
                    return Direction.Left;
                case Direction.Left:
                    return Direction.Right;
            }

            throw new ArgumentOutOfRangeException("direction not supported");
        }
    }
}
