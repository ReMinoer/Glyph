using Microsoft.Xna.Framework;
using Simulacra.Utils;

namespace Glyph.Space
{
    public interface IWriteableGrid<T> : IGrid<T>, IWriteableArray<T>
    {
        T this[int i, int j] { get; set; }
        T this[Point gridPoint] { get; set; }
        T this[Vector2 worldPoint] { get; set; }
        T this[IGridPositionable gridPositionable] { get; set; }
    }
}