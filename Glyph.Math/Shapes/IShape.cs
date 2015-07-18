using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public interface IShape
    {
        bool Contains(Vector2 point);
    }
}