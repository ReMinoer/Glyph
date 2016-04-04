using System;
using Glyph.Composition;
using Glyph.Math;

namespace Glyph.Physics.Colliders
{
    public interface ICollider : IShape, IEnableable, IUpdate
    {
        event Action<Collision> Collided;
        bool IsColliding(ICollider collider, out Collision collision);
        bool IsColliding(RectangleCollider collider, out Collision collision);
        bool IsColliding(CircleCollider collider, out Collision collision);
        bool IsColliding(IGridCollider collider, out Collision collision);
    }

    public interface ICollider<out TShape> : ICollider, IShapedObject<TShape>
        where TShape : IShape
    {
    }
}