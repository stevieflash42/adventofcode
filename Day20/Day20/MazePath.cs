using System;
using System.Collections.Generic;
using Day20.Enums;

namespace Day20
{
    public class MazePath
    {
        public Direction LastDirectionMoved { get; set; }
        public bool MadeToEnd { get; private set; }
        public bool Finished { get; private set; }
        public int Steps { get; private set; }
        public List<(int x, int y)> PreviouslyVisitedLocations { get; private set; }

        public MazePath(Direction startingMovementDirection, int nSteps, List<(int x, int y)> previouslyVisitedLocations)
        {
            this.LastDirectionMoved = startingMovementDirection;
            this.Steps = nSteps;
            this.PreviouslyVisitedLocations = previouslyVisitedLocations ?? new List<(int x, int y)>();
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
        }
    }
}