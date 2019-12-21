using Day20.Enums;

namespace Day20
{
    public class PortalPiece : MazeElement
    {
        public char PartName { get; }

        public PortalPiece(MazeElementType type, char charElement, int x, int y) : base(type, x, y)
        {
            this.PartName = charElement;
        }
    }
}