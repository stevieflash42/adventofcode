using System;
using System.Collections.Generic;
using System.Text;
using Day20.Enums;

namespace Day20
{
    public class MazePath
    {
        public Direction LastDirectionMoved { get; set; }
        public bool MadeToEnd { get; private set; }
        public bool Finished { get; private set; }
        public int Steps { get; private set; }

        public MazePath()
        {
        }

        public MazePath(MazePath originalPath)
        {
            if (originalPath.Finished)
            {
                throw new ArgumentOutOfRangeException("originalPath cannot be finished");
            }
            this.Steps = originalPath.Steps;
            this.LastDirectionMoved = originalPath.LastDirectionMoved;
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