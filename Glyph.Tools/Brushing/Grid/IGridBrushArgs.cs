using Microsoft.Xna.Framework;

namespace Glyph.Tools.Brushing.Grid
{
    public interface IGridBrushArgs
    {
        Point GridPoint { get; }
        Vector2 WorldPoint { get; }
    }
}