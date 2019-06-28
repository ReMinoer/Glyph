using Microsoft.Xna.Framework;

namespace Glyph.Tools.Brushing.Grid
{
    public class GridBrushArgs : IGridBrushArgs
    {
        public Point Point { get; set; }
        public GridBrushArgs() {}
        public GridBrushArgs(Point point) => Point = point;
    }
}