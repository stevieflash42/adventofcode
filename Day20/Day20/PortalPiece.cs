using Day20.Enums;

namespace Day20
{
    public class PortalPiece : MazeElement
    {
        public char PartName { get; }

        public PortalPiece(MazeElementType type, char charElement, int x, int y, bool isOuterEdge) : base(type, x, y, isOuterEdge)
        {
            this.PartName = charElement;
        }
    }
}