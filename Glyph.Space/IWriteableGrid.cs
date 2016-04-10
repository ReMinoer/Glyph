using System;
using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public interface IWriteableGrid<T> : IGrid<T>
    {
        new T this[int i, int j] { get; set; }
        new T this[Point gridPoint] { get; set; }
        new T this[Vector2 worldPoint] { get; set; }
        new T this[IGridPositionable gridPositionable] { get; set; }
        Func<T> OutOfBoundsValueFactory { get; }
    }
}