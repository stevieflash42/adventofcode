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
            this.Rows = arStrMaze.Select((strRow, y) => new MazeRow(strRow, y, y == arStrMaze.Length - 1)).ToArray();
            this.Portals = BuildMazePortals(this);
        }

        public MazeRow this[int y] => (this.Rows.Length <= y) ? null : this.Rows[y];

        public MazeElement GetLocation(int x, int y) => this[y]?[x];

        //caching to avoid wasted cycles
        private Portal _aa;
        public (int x, int y, Direction startingMoveDirection) FindStartPositionAndMovementDirection()
        {
            if (null != _aa) return (_aa.InOutTile.X, _aa.InOutTile.Y, _aa.InOutRelationTo);
            _aa = this.Portals.FirstOrDefault(portal => portal.Name == "AA");
            if (null == _aa)
            {
                throw new NullReferenceException("bad data - portal AA doesn't exist");
            }

            return (_aa.InOutTile.X, _aa.InOutTile.Y, _aa.InOutRelationTo);
        }

        //caching to avoid wasted cycles
        private Portal _zz;
        public (int x, int y) FindEndPosition()
        {
            if (null != _zz) return (_zz.InOutTile.X, _zz.InOutTile.Y);
            _zz = this.Portals.FirstOrDefault(portal => portal.Name == "ZZ");
            if (null == _zz)
            {
                throw new NullReferenceException("bad data - portal ZZ doesn't exist");
            }

            return (_zz.InOutTile.X, _zz.InOutTile.Y);
        }

        public List<Direction> DetermineMoveableDirectionsFromPosition(int x, int y)
        {
            MazeElement current = GetLocation(x, y);
            if (null == current)
            {
                throw new ArgumentOutOfRangeException("location does not exist");
            }

            MazeElement up = GetLocation(x, y - 1);
            MazeElement down = GetLocation(x, y + 1);
            MazeElement left = GetLocation(x - 1, y);
            MazeElement right = GetLocation(x + 1, y);

            List<Direction> theReturn = new List<Direction>();
            if (CanWalkOnElement(up)) theReturn.Add(Direction.Up);
            if (CanWalkOnElement(down)) theReturn.Add(Direction.Down);
            if (CanWalkOnElement(left)) theReturn.Add(Direction.Left);
            if (CanWalkOnElement(right)) theReturn.Add(Direction.Right);
            return theReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="portalOutput">exitPortal and entrancePortal won't be null when bUsedPortal is true</param>
        /// <returns></returns>
        public (int x, int y) DeterminePositionAfterMovingToLocation(int x, int y,
            out (bool bUsedPortal, Portal exitPortal, Portal entrancePortal) portalOutput)
        {
            MazeElement current = GetLocation(x, y);
            if (null == current)
            {
                throw new ArgumentOutOfRangeException("location does not exist");
            }

            if (!CanWalkOnElement(current))
            {
                throw new ArgumentOutOfRangeException("cannot walk on element");
            }

            switch (current.Type)
            {
                //if it's an open passage, then you're on the same spot
                case MazeElementType.OpenPassage:
                    portalOutput = (false, null, null);
                    return (x, y);
                //if you're moving onto a portal piece then you need to be on the exit of the matching portal
                case MazeElementType.PortalPiece:
                    Portal entrancePortal = FindPortalForPiece(current as PortalPiece);
                    Portal exitPortal = FindExitPortal(entrancePortal);
                    MazeElement exitTile = exitPortal.InOutTile;
                    if (null == exitTile)
                    {
                        throw new ArgumentOutOfRangeException("could not locate exit tile");
                    }

                    portalOutput = (true, exitPortal, entrancePortal);
                    return (exitTile.X, exitTile.Y);
            }

            throw new ArgumentOutOfRangeException("unsupported tile type");
        }

        private Portal FindPortalForPiece(PortalPiece portalPiece) =>
            this.Portals.FirstOrDefault(portal => portal.ContainsPortalPiece(portalPiece));

        private Portal FindExitPortal(Portal entrancePortal)
        {
            if (null == entrancePortal) return null;
            return this.Portals.FirstOrDefault(portal =>
                !entrancePortal.Equals(portal) && portal.Name == entrancePortal.Name);
        }

        private static bool CanWalkOnElement(MazeElement elm)
        {
            if (null == elm) return false;
            //you're only allowed to walk on open passage locations and portal pieces
            // - assumption: technically there's 1 of the portal pieces you can't walk on, we're assuming the data is good and you should never get to it anyway
            return elm.Type == MazeElementType.OpenPassage || elm.Type == MazeElementType.PortalPiece;
        }

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