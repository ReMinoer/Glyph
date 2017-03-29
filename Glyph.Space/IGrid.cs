using System.Collections.Generic;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public interface IGrid : IShape, IEnumerable<Point>
    {
        GridDimension Dimension { get; }
        Rectangle Bounds { get; }
        Vector2 Delta { get; }
        Vector2 ToWorldPoint(int i, int j);
        Vector2 ToWorldPoint(Point gridPoint);
        Vector2 ToWorldPoint(IGridPositionable gridPoint);
        TopLeftRectangle ToWorldRange(int x, int y, int width, int height);
        TopLeftRectangle ToWorldRange(Rectangle rectangle);
        Point ToGridPoint(Vector2 worldPoint);
        Rectangle ToGridRange(TopLeftRectangle rectangle);
        bool ContainsPoint(int i, int j);
        bool ContainsPoint(Point gridPoint);
    }

    public interface IGrid<out T> : IGrid
    {
        bool HasLowEntropy { get; }
        IEnumerable<T> Values { get; }
        IEnumerable<IGridCase<T>> SignificantCases { get; }
        T this[int i, int j] { get; }
        T this[Point gridPoint] { get; }
        T this[Vector2 worldPoint] { get; }
        T this[IGridPositionable gridPositionable] { get; }
        T[][] ToArray();
    }
}