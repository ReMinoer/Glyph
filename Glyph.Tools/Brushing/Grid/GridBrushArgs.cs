using Microsoft.Xna.Framework;

namespace Glyph.Tools.Brushing.Grid
{
    public struct GridBrushArgs : IGridBrushArgs
    {
        public Point GridPoint { get; set; }
        public Vector2 WorldPoint { get; set; }
        
        public GridBrushArgs(Point gridPoint, Vector2 worldPoint)
        {
            GridPoint = gridPoint;
            WorldPoint = worldPoint;
        }
    }
}