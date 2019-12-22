using System;
using System.Collections.Generic;
using System.Text;

namespace Day20
{
    public class MazePath
    {
        public int Steps { get; private set; }

        public MazePath()
        {
        }

        public MazePath(MazePath originalPath) => this.Steps = originalPath.Steps;

        public void Increment() => this.Steps++;
    }
}