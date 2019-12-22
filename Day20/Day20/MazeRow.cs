using System.Linq;

namespace Day20
{
    public class MazeRow
    {
        public MazeElement[] Elements { get; }

        public MazeRow(string strRow, int y)
        {
            this.Elements = ParseRowElements(strRow, y);
        }

        private static MazeElement[] ParseRowElements(string strRow, int y) =>
            strRow.ToCharArray().Select((charElement, x) => MazeElement.GetMazeElement(charElement, x, y))
                .ToArray();

        public MazeElement this[int x] => (this.Elements.Length <= x) ? null : this.Elements[x];
    }
}
