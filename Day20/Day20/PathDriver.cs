using System;
using System.Collections.Generic;
using System.Linq;
using Day20.Enums;

namespace Day20
{
    public class PathDriver
    {
        private Maze Maze { get; }

        public PathDriver(Maze maze)
        {
            this.Maze = maze ?? throw new NullReferenceException("maze");
        }

        public int DetermineShortestPathThroughMaze()
        {
            (int startX, int startY, Direction startingMoveDirection) =
                this.Maze.FindStartPositionAndMovementDirection();
            MazePath startPath = new MazePath(startingMoveDirection, 0);
            List<MazePath> paths = new List<MazePath> {startPath};
            DeterminePathsFromLocation(startX, startY, startPath, paths);
            return paths.Where(path => path.Finished && path.MadeToEnd).Min(path => path.Steps);
        }

        private (int newX, int newY) GetNewLocation(int x, int y, Direction direction,
            out Direction actualPreviousDirection)
        {
            (int, int) theReturn = (x, y - 1);
            switch (direction)
            {
                case Direction.Up:
                    theReturn = (x, y - 1);
                    break;
                case Direction.Down:
                    theReturn = (x, y + 1);
                    break;
                case Direction.Left:
                    theReturn = (x - 1, y);
                    break;
                case Direction.Right:
                    theReturn = (x + 1, y);
                    break;
            }

            theReturn = this.Maze.DeterminePositionAfterMovingToLocation(theReturn.Item1, theReturn.Item2,
                out (bool bUsedPortal, Portal thePortal) portalOutput);

            actualPreviousDirection = portalOutput.bUsedPortal ? portalOutput.thePortal.InOutRelationTo : direction;

            return theReturn;
        }

        private void DeterminePathsFromLocation(int x, int y, MazePath currentPath, List<MazePath> allPathsTaken)
        {
            //if we're at the end then terminate
            if (this.Maze.FindEndPosition() == (x, y))
            {
                currentPath.SetFinished(true);
                return;
            }

            (int startX, int startY, Direction startingMoveDirection) =
                this.Maze.FindStartPositionAndMovementDirection();
            //there's a nasty case where the path can loop back around - we want to prevent this from happening
            if (startX == x && startY == y && startingMoveDirection != currentPath.LastDirectionMoved)
            {
                currentPath.SetFinished(false);
                return;
            }

            List<Direction> directions = this.Maze.DetermineMoveableDirectionsFromPosition(x, y);
            //we don't want to travel the direction we just came from
            Direction directionWeJustCameFrom = DirectionHelper.GetOppositeDirection(currentPath.LastDirectionMoved);
            directions = directions.Where(direction => directionWeJustCameFrom != direction).ToList();

            int nLastSteps = currentPath.Steps;
            Direction lastDirection = currentPath.LastDirectionMoved;
            switch (directions.Count)
            {
                //dead end
                case 0:
                    //humoring Murphy, but I'm pretty sure that it's impossible for this to be true
                    currentPath.SetFinished(this.Maze.FindEndPosition() == (x, y));
                    break;
                case 1:
                case 2:
                case 3:
                    for (var i = 0; i < directions.Count; i++)
                    {
                        Direction direction = directions[i];
                        if (i != 0)
                        {
                            MazePath newPath = new MazePath(lastDirection, nLastSteps);
                            allPathsTaken.Add(newPath);
                            newPath.Increment();
                            (int newX, int newY) = GetNewLocation(x, y, direction, out direction);
                            newPath.LastDirectionMoved = direction;
                            DeterminePathsFromLocation(newX, newY, newPath, allPathsTaken);
                        }
                        else
                        {
                            currentPath.Increment();
                            (int newX, int newY) = GetNewLocation(x, y, directions[0], out direction);
                            currentPath.LastDirectionMoved = direction;
                            DeterminePathsFromLocation(newX, newY, currentPath, allPathsTaken);
                        }
                    }

                    break;
                default:
                    throw new IndexOutOfRangeException("bad data - should not have more than 3 directions");
            }
        }
    }
}