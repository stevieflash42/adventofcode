using System;
using Day20.Enums;

namespace Day20
{
    public class Portal
    {
        public MazeElement InOutTile { get; }
        public string Name { get; }
        public Direction InOutRelationTo { get; }
        private PortalPiece p1;
        private PortalPiece p2;
        public Portal(PortalPiece a1, PortalPiece a2, MazeElement inOutTile)
        {
            if (inOutTile.Type != MazeElementType.OpenPassage)
            {
                throw new ArgumentOutOfRangeException("inOutTile");
            }

            this.Name = DeterminePortalName(a1, a2);
            this.InOutTile = inOutTile;
            this.p1 = a1;
            this.p2 = a2;
            this.InOutRelationTo = GetElmRelationshipToPortalPiece(this.p1, this.InOutTile);
        }

        public bool ContainsPortalPiece(PortalPiece portalPiece) =>
            this.p1.Equals(portalPiece) || this.p2.Equals(portalPiece);

        /// <summary>
        /// assumption: a1 and a2 are NOT the same, they're NOT in the same cardinal location, and they're adjacent
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        private static string DeterminePortalName(PortalPiece a1, PortalPiece a2)
        {
            if (a1.X > a2.X)
            {
                return $"{a2.PartName}{a1.PartName}";
            }

            if (a2.X > a1.X)
            {
                return $"{a1.PartName}{a2.PartName}";
            }

            if (a1.Y > a2.Y)
            {
                return $"{a2.PartName}{a1.PartName}";
            }

            if (a2.Y > a1.Y)
            {
                return $"{a1.PartName}{a2.PartName}";
            }

            throw new InvalidOperationException(
                "Could not determine the relationship between the portal pieces in order to name the portal");
        }

        /// <summary>
        /// assumption: portalPiece and elm are NOT the same, they're NOT in the same cardinal location
        /// </summary>
        /// <param name="portalPiece">Doesn't matter which portalPiece you use. Assumption: the portal pieces are adjacent</param>
        /// <param name="elm"></param>
        /// <returns></returns>
        private static Direction GetElmRelationshipToPortalPiece(PortalPiece portalPiece, MazeElement elm)
        {
            if (portalPiece.X > elm.X)
            {
                return Direction.Left;
            }

            if (elm.X > portalPiece.X)
            {
                return Direction.Right;
            }

            if (portalPiece.Y > elm.Y)
            {
                return Direction.Up;
            }

            if (elm.Y > portalPiece.Y)
            {
                return Direction.Down;
            }

            throw new InvalidOperationException(
                "Could not determine the relationship between the portal piece and the element");
        }
    }
}
