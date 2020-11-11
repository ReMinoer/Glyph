using System.Collections.Generic;
using Glyph.Math;
using Microsoft.Xna.Framework;
using Simulacra.Utils;

namespace Glyph.Space
{
    public interface IGrid : IArrayDefinition, IShape
    {
        GridDimension Dimension { get; }
        Vector2 Delta { get; }
        ITransformation Transformation { get; set; }
        Vector2 ToWorldPoint(Point gridPoint);
        Point ToGridPoint(Vector2 worldPoint);
    }

    public interface IGrid<out T> : IGrid, ITwoDimensionArray<T>, INotifyArrayChanged
    {
        bool HasLowEntropy { get; }
        IEnumerable<T> Values { get; }
        IEnumerable<IGridCase<T>> SignificantCases { get; }
        T this[Point gridPoint] { get; }
        T this[Vector2 worldPoint] { get; }
    }
}