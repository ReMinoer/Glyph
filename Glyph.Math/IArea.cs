using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Math
{
    public interface IArea
    {
        IRectangle BoundingBox { get; }
        bool ContainsPoint(Vector2 point);
    }
}