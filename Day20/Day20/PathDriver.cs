using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Day20.Enums;
using Microsoft.Win32.SafeHandles;

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
            (int startX, int startY) = this.Maze.FindStartPosition();
            MazePath startPath = new MazePath();
            List<MazePath> paths = new List<MazePath> {startPath};
            DeterminePathsFromLocation(startX, startY, startPath, paths);
            return paths.Where(path => path.Finished && path.MadeToEnd).Min(path => path.Steps);
        }

        private static (int newX, int newY) GetNewLocation(int x, int y, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return (x, y - 1);
                case Direction.Down:
                    return (x, y + 1);
                case Direction.Left:
                    return (x - 1, y);
                case Direction.Right:
                    return (x + 1, y);
            }

            throw new ArgumentOutOfRangeException("direction not supported");
        }
    
        private void DeterminePathsFromLocation(int x, int y, MazePath currentPath, List<MazePath> allPathsTaken)
        {
            //if we're at the end then terminate
            if (this.Maze.FindEndPosition() == (x, y))
            {
                currentPath.SetFinished(true);
                return;
            }

            List<Direction> directions = this.Maze.DetermineMoveableDirectionsFromPosition(x, y);
            //we don't want to travel the direction we just came from
            Direction directionWeJustCameFrom = DirectionHelper.GetOppositeDirection(currentPath.LastDirectionMoved);
            directions = directions.Where(direction => directionWeJustCameFrom != direction).ToList();

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
                            MazePath newPath = new MazePath(currentPath);
                            allPathsTaken.Add(newPath);
                            newPath.Increment();
                            (int newX, int newY) = GetNewLocation(x, y, direction);
                            DeterminePathsFromLocation(newX, newY, newPath, allPathsTaken);
                        }
                        else
                        {
                            currentPath.Increment();
                            (int newX, int newY) = GetNewLocation(x, y, directions[0]);
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