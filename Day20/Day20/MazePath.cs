using System;
using System.Collections.Generic;
using System.Linq;
using Day20.Enums;

namespace Day20
{
    public class MazePath
    {
        //private int ID = new Random().Next(0, 1000);
        private int _level;

        public int Level
        {
            get => _level;
            set
            {
                if (this._level == value) return;
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "level cannot go below 0; make sure you're only using portal AA and ZZ, OR INNER portals on level 0; all other portals are walls");
                }

                _level = value;
            }
        }

        public Direction LastDirectionMoved { get; set; }
        public bool MadeToEnd { get; private set; }
        public bool Finished { get; private set; }
        public int Steps { get; private set; }
        public List<(int x, int y)> PreviouslyVisitedLocations { get; private set; }
        public List<Direction> Movements { get; private set; }

        public MazePath(Direction startingMovementDirection, int nSteps, int nLevel, List<(int x, int y)> previouslyVisitedLocations, List<Direction> movements)
        {
            this.LastDirectionMoved = startingMovementDirection;
            this.Steps = nSteps;
            this.PreviouslyVisitedLocations = previouslyVisitedLocations ?? new List<(int x, int y)>();
            this.Level = nLevel;
            this.Movements = movements ?? new List<Direction>();
        }

        public void Increment()
        {
            if (this.Finished) throw new MethodAccessException("Cannot call increment on a path that's finished");
            this.Steps++;
        }

        public void SetFinished(bool bMadeToEnd)
        {
            this.MadeToEnd = bMadeToEnd;
            this.Finished = true;
            //Console.WriteLine($"Ended at Level {this.Level} with {this.Steps} Steps");
        }

        public override string ToString()
        {
            return string.Join(" => ", this.Movements.Select(mov => $"{mov.ToString()}"));
            //return string.Join(" => ", this.PreviouslyVisitedLocations.Select(loc => $"({loc.x}, {loc.y})"));
        }
    }
}