using Microsoft.Xna.Framework;
using Simulacra.Utils;

namespace Glyph.Space
{
    public interface IWriteableGrid<T> : IGrid<T>, ITwoDimensionWriteableArray<T>
    {
        new T this[Point gridPoint] { get; set; }
        new T this[Vector2 worldPoint] { get; set; }
    }
}