using Glyph.Composition;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Physics.Colliders
{
    public interface ICollider : ILoadContent, IDraw
    {
        bool Intersects(RectangleCollider collider);
        bool Intersects(CircleCollider collider);
        bool ContainsPoint(Vector2 point);
    }

    public interface ICollider<out TShape> : ICollider
        where TShape : IShape
    {
        TShape Shape { get; }
    }
}