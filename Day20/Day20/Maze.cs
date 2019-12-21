using System;
using System.Collections.Generic;
using System.Linq;
using Day20.Enums;

namespace Day20
{
    public class Maze
    {
        public MazeRow[] Rows { get; }
        public Portal[] Portals { get; }

        public Maze(string[] arStrMaze)
        {
            this.Rows = arStrMaze.Select((strRow, y) => new MazeRow(strRow, y)).ToArray();
            this.Portals = BuildMazePortals(this);
        }

        public MazeRow this[uint y] => (this.Rows.Length <= y) ? null : this.Rows[y];

        private static Portal[] BuildMazePortals(Maze maze)
        {
            List<Portal> theReturn = new List<Portal>();

            List<MazeElement> allElements = maze.Rows.SelectMany(row => row.Elements).ToList();

            List<PortalPiece> portalPieces = allElements.Where(elm => elm.Type == MazeElementType.PortalPiece)
                .Select(elm => elm as PortalPiece).ToList();

            List<PortalPiece> alreadyProcessed = new List<PortalPiece>();
            foreach (PortalPiece piece in portalPieces)
            {
                if (alreadyProcessed.Contains(piece)) continue;
                PortalPiece adj = portalPieces.FirstOrDefault(p => MazeElement.AreElementsAdjacent(piece, p));
                if (null == adj)
                {
                    throw new Exception("bad data - portal pieces must be adjacent");
                }

                alreadyProcessed.Add(piece);
                alreadyProcessed.Add(adj);

                //assumption: portal will have a single OpenPassage near it
                var nearestOpening = allElements.Where(elm => elm.Type == MazeElementType.OpenPassage)
                    .FirstOrDefault(elm =>
                        MazeElement.AreElementsAdjacent(adj, elm) || MazeElement.AreElementsAdjacent(piece, elm));

                if (null == nearestOpening)
                {
                    throw new Exception("bad data - there must be an opening near one of the portal pieces");
                }

                theReturn.Add(new Portal(piece, adj, nearestOpening));
            }

            return theReturn.ToArray();
        }
    }
}
