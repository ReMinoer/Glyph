using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public interface IShape
    {
        Vector2 Center { get; set; }
        bool Intersects(IRectangle collider);
        bool Intersects(ICircle collider);
        bool ContainsPoint(Vector2 point);
    }
}