using System;

namespace Day20
{
    class Program
    {
        static void Main(string[] args)
        {
            Maze maze = BuildMazeFromFile("Input.txt");
            Console.WriteLine($"Shorted path is: {new PathDriver(maze).DetermineShortestPathThroughMaze()}");
            Console.Read();
        }

        private static Maze BuildMazeFromFile(string strFileName) => new Maze(System.IO.File.ReadAllLines(strFileName));
    }
}