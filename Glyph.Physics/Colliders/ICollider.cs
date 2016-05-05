using System;
using Glyph.Composition;
using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Physics.Colliders
{
    public interface ICollider : IShape, IEnableable, IUpdate
    {
        bool IsFreeze { get; set; }
        Vector2 LocalCenter { get; set; }
        Predicate<ICollider> IgnoredFilter { get; set; }
        event Action<Collision> Colliding;
        event Action<Collision> Collided;
        bool IsColliding(ICollider collider, out Collision collision);
    }

    public interface ICollider<out TShape> : ICollider, IShapedObject<TShape>
        where TShape : IShape
    {
    }
}