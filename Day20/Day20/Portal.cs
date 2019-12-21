using System;
using Day20.Enums;

namespace Day20
{
    public class Portal
    {
        public string Name { get; }
        public Portal(PortalPiece a1, PortalPiece a2, MazeElement inOutTile)
        {
            if (inOutTile.Type != MazeElementType.OpenPassage)
            {
                throw new ArgumentOutOfRangeException("inOutTile");
            }

            this.Name = DeterminePortalName(a1, a2);
        }

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
    }
}
