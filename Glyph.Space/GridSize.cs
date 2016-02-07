using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public struct GridSize
    {
        public int Columns { get; set; }
        public int Rows { get; set; }

        public GridSize(int columns, int rows)
            : this()
        {
            Columns = columns;
            Rows = rows;
        }

        static public implicit operator Point(GridSize gridSize)
        {
            return new Point(gridSize.Columns, gridSize.Rows);
        }

        static public implicit operator Vector2(GridSize gridSize)
        {
            return new Vector2(gridSize.Columns, gridSize.Rows);
        }
    }
}