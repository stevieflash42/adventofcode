using System;
using Day20.Enums;

namespace Day20
{
    public class MazeElement
    {
        public MazeElementType Type { get; }
        public int X { get; }
        public int Y { get; }

        public MazeElement(MazeElementType type, int x, int y)
        {
            this.Type = type;
            this.X = x;
            this.Y = y;
        }

        private static MazeElementType DetermineMazeElementType(char charElement)
        {
            switch (charElement)
            {
                case ' ':
                    return MazeElementType.EmptySpace;
                case '#':
                    return MazeElementType.SolidWall;
                case '.':
                    return MazeElementType.OpenPassage;
                default:
                    return MazeElementType.PortalPiece;
            }
        }

        public static MazeElement GetMazeElement(char charElement, int x, int y)
        {
            MazeElementType type = DetermineMazeElementType(charElement);
            switch (type)
            {
                case MazeElementType.PortalPiece:
                    return new PortalPiece(type, charElement, x, y);
                default:
                    return new MazeElement(type, x, y);
            }
        }

        public static bool AreElementsAdjacent(MazeElement a, MazeElement b)
        {
            return (Math.Abs(a.X - b.X) == 1 && Math.Abs(a.Y - b.Y) == 0) ||
                   (Math.Abs(a.X - b.X) == 0 && Math.Abs(a.Y - b.Y) == 1);
        }

        public override bool Equals(object obj)
        {
            if (null == obj) return false;
            if (obj.GetType() != this.GetType()) return false;
            if (obj is MazeElement mazeElm)
            {
                return this.X == mazeElm.X && this.Y == mazeElm.Y;
            }

            return obj.Equals(this);
        }
    }
}