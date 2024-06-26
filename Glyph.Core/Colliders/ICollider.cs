﻿using System;
using Glyph.Composition;
using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Core.Colliders
{
    public interface ICollider : IMovableShape, IUpdate
    {
        new bool Active { get; }
        bool IsFreeze { get; set; }
        bool IsImmovable { get; set; }
        bool Unphysical { get; set; }
        SceneNode ParentNode { get; }
        Vector2 LocalCenter { get; set; }
        Predicate<ICollider> IgnoredFilter { get; set; }
        event Action<Collision> Colliding;
        event Action<Collision> Collided;
        bool IsColliding(ICollider collider, out Collision collision);
    }

    public interface ICollider<out TShape> : ICollider, IShapedComponent<TShape>
        where TShape : IShape
    {
    }
}