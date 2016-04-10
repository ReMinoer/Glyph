using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public interface IGridCase<out T>
    {
        Point Point { get; }
        T Value { get; }
    }
}