using Microsoft.Xna.Framework;
using Simulacra.Utils;

namespace Glyph.Space
{
    public interface IWriteableGrid<T> : IGrid<T>, IWriteableArray<T>
    {
        new T this[int i, int j] { get; set; }
        new T this[Point gridPoint] { get; set; }
        new T this[Vector2 worldPoint] { get; set; }
        new T this[IGridPositionable gridPositionable] { get; set; }
    }
}