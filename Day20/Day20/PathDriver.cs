using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            MazePath startPath = new MazePath(startingMoveDirection, 0, 0, null/*, null*/);
            List<MazePath> paths = new List<MazePath> {startPath};
            //https://stackoverflow.com/a/4513507
            Thread thread = new Thread(() => { DeterminePathsFromLocation(startX, startY, startPath, paths); },
                int.MaxValue);
            thread.Start();
            thread.Join();
            //string strOutput = string.Join("\n\n", paths.Select(path => path.ToString()));
            //MazePath whatICareAbout = paths.FirstOrDefault(path =>
            //    path.ToString()
            //        .Contains(
            //            "Up => Up => Left => Left => Left => Left => Left => Left => Up => Up => Up => Up => Right => Right => Right => Right => Right => Right => Up"));
            return paths.Where(path => path.Finished && path.MadeToEnd).Min(path => path.Steps);
        }

        private (int newX, int newY, int nLevelIncrement) GetNewLocation(int x, int y, Direction direction,
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
                out (bool bUsedPortal, Portal exitPortal, Portal entrancePortal) portalOutput);

            actualPreviousDirection = portalOutput.bUsedPortal ? portalOutput.exitPortal.InOutRelationTo : direction;

            //you don't increment if you haven't gone through a portal
            int nLevelIncrement = 0;
            if (portalOutput.bUsedPortal)
            {
                nLevelIncrement = portalOutput.entrancePortal.IsOuterPortal ? -1 : 1;
            }

            return (theReturn.Item1, theReturn.Item2, nLevelIncrement);
        }

        private void DeterminePathsFromLocation(int x, int y, MazePath currentPath, List<MazePath> allPathsTaken)
        {
            //if we're at the end then terminate
            if (this.Maze.FindEndPosition() == (x, y))
            {
                currentPath.SetFinished(currentPath.Level == 0);
                return;
            }

            (int startX, int startY, Direction startingMoveDirection) =
                this.Maze.FindStartPositionAndMovementDirection();
            //there's a nasty case where the path can loop back around - we want to prevent this from happening
            if (startX == x && startY == y && startingMoveDirection != currentPath.LastDirectionMoved)
            {
                currentPath.SetFinished(false);
                currentPath.SetFinished(currentPath.Level == 0);
                return;
            }

            //we're going to blow the recursive stack soon - just terminate
            if (currentPath.Steps > 10000000)
            {
                currentPath.SetFinished(false);
                return;
            }

            Console.WriteLine(allPathsTaken.Count);

            ////terminate... we're getting out of control
            //if (allPathsTaken.Count > 100000)
            //{
            //    currentPath.SetFinished(false);
            //    return;
            //}

            //if we've managed to find a path after all this time then terminate
            if (null != allPathsTaken.FirstOrDefault(path => path.Finished && path.MadeToEnd))
            {
                currentPath.SetFinished(false);
                return;
            }

            List<Direction> directions = this.Maze.DetermineMoveableDirectionsFromPosition(x, y);
            //we don't want to travel the direction we just came from
            Direction directionWeJustCameFrom = DirectionHelper.GetOppositeDirection(currentPath.LastDirectionMoved);
            directions = directions.Where(direction =>
            {
                bool validDirection = directionWeJustCameFrom != direction;
                if (!validDirection) return false;
                //because we've written this declaratively we're validating the directions up here and it's causing multiple iterations of this calculation
                // - todo: find a better way to do this?
                (int _, int _, int nLevelIncrement) = GetNewLocation(x, y, direction, out direction);
                //we can't go this direction because level movements below 0 aren't possible
                return nLevelIncrement >= 0 || currentPath.Level != 0;
            }).ToList();

            int nLastLevel = currentPath.Level;
            int nLastSteps = currentPath.Steps;
            Direction lastDirection = currentPath.LastDirectionMoved;
            //we want to clone the instance so that it's not affected by the iterative-recursive runs
            // - yes, that's a terrifying term: iterative-recursive
            List<(int x, int y, int level)> previouslyVisitedLocations =
                new List<(int x, int y, int level)>(currentPath.PreviouslyVisitedLocations);
            //List<Direction> movements = new List<Direction>(currentPath.Movements);
            switch (directions.Count)
            {
                //dead end
                case 0:
                    //humoring Murphy, but I'm pretty sure that it's impossible for this to be true
                    currentPath.SetFinished(this.Maze.FindEndPosition() == (x, y) && currentPath.Level == 0);
                    break;
                case 1:
                case 2:
                case 3:
                    for (int i = 0; i < directions.Count; i++)
                    {
                        Direction direction = directions[i];
                        (int newX, int newY, int nLevelIncrement) = GetNewLocation(x, y, direction, out direction);
                        //if we've been to this location before on this path then terminate the path
                        if (currentPath.PreviouslyVisitedLocations.Contains((newX, newY,
                                currentPath.Level + nLevelIncrement))
                            || null != allPathsTaken.FirstOrDefault(path =>
                                path.PreviouslyVisitedLocations.Contains((newX, newY,
                                    currentPath.Level + nLevelIncrement))))
                        {
                            currentPath.SetFinished(false);
                            //we want to CONTINUE and NOT RETURN because the other directions might be valid
                            continue;
                        }

                        if (i != 0)
                        {
                            MazePath newPath = new MazePath(lastDirection, nLastSteps, nLastLevel,
                                previouslyVisitedLocations/*, movements*/);
                            allPathsTaken.Add(newPath);
                            newPath.Increment();
                            newPath.Level += nLevelIncrement;
                            newPath.LastDirectionMoved = direction;
                            newPath.PreviouslyVisitedLocations.Add((newX, newY, newPath.Level));
                            //newPath.Movements.Add(direction);
                            DeterminePathsFromLocation(newX, newY, newPath, allPathsTaken);
                        }
                        else
                        {
                            currentPath.Increment();
                            currentPath.Level += nLevelIncrement;
                            currentPath.LastDirectionMoved = direction;
                            currentPath.PreviouslyVisitedLocations.Add((newX, newY, currentPath.Level));
                            //currentPath.Movements.Add(direction);
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