using System;
using Glyph.Composition;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Physics.Colliders
{
    public interface ICollider : IEnableable, ILoadContent, IUpdate, IDraw, IDisposable
    {
        Vector2 Center { get; set; }
        event Action<Collision> Collided;
        bool IsColliding(ICollider collider, out Collision collision);
        bool IsColliding(RectangleCollider collider, out Collision collision);
        bool IsColliding(CircleCollider collider, out Collision collision);
    }

    public interface ICollider<out TShape> : ICollider
        where TShape : IShape
    {
        TShape Shape { get; }
    }
}