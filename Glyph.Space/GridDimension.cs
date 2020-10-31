using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public struct GridDimension
    {
        public int Columns { get; set; }
        public int Rows { get; set; }

        public GridDimension(int columns, int rows)
            : this()
        {
            Columns = columns;
            Rows = rows;
        }

        public int[] ToArray() => new[] {Rows, Columns};

        static public implicit operator Point(GridDimension gridDimension)
        {
            return new Point(gridDimension.Columns, gridDimension.Rows);
        }

        static public implicit operator Vector2(GridDimension gridDimension)
        {
            return new Vector2(gridDimension.Columns, gridDimension.Rows);
        }
    }
}