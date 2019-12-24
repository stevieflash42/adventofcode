using System.Linq;

namespace Day20
{
    public class MazeRow
    {
        public MazeElement[] Elements { get; }

        public MazeRow(string strRow, int y, bool bIsOuterEdge)
        {
            this.Elements = ParseRowElements(strRow, y, bIsOuterEdge);
        }

        private static MazeElement[] ParseRowElements(string strRow, int y, bool bIsOuterEdge) =>
            strRow.ToCharArray().Select((charElement, x) => MazeElement.GetMazeElement(charElement, x, y,
                    y == 0 || bIsOuterEdge || x == 0 || x == strRow.Length - 1))
                .ToArray();

        public MazeElement this[int x] => (this.Elements.Length <= x) ? null : this.Elements[x];
    }
}
