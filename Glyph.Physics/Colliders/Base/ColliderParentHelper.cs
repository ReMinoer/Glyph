﻿using System;
using System.Linq;
using Glyph.Composition;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Physics.Colliders.Base
{
    internal class ColliderParentImplementation
    {
        private readonly IGlyphParent<ICollider> _colliderParent;
        public bool Enabled { get; set; }
        public Predicate<ICollider> IgnoredFilter { get; set; }

        public ColliderParentImplementation(IGlyphParent<ICollider> colliderParent)
        {
            _colliderParent = colliderParent;
        }

        public Vector2 Center
        {
            get
            {
                int i = 0;
                Vector2 center = Vector2.Zero;
                foreach (ICollider collider in _colliderParent.Components)
                {
                    center += collider.Center;
                    i++;
                }
                return center / i;
            }
            set
            {
                Vector2 center = Center;
                foreach (ICollider collider in _colliderParent.Components)
                    collider.Center = collider.Center - center + value;
            }
        }

        public Vector2 LocalCenter
        {
            get
            {
                int i = 0;
                Vector2 localCenter = Vector2.Zero;
                foreach (ICollider collider in _colliderParent.Components)
                {
                    localCenter += collider.LocalCenter;
                    i++;
                }
                return localCenter / i;
            }
            set
            {
                Vector2 localCenter = LocalCenter;
                foreach (ICollider collider in _colliderParent.Components)
                    collider.LocalCenter = collider.LocalCenter - localCenter + value;
            }
        }

        public IRectangle BoundingBox
        {
            get
            {
                float top = float.MaxValue;
                float bottom = float.MinValue;
                float left = float.MaxValue;
                float right = float.MinValue;

                foreach (ICollider collider in _colliderParent.Components)
                {
                    IRectangle boundingBox = collider.BoundingBox;

                    if (boundingBox.Top < top)
                        top = boundingBox.Top;
                    if (boundingBox.Bottom > bottom)
                        bottom = boundingBox.Bottom;
                    if (boundingBox.Left < left)
                        left = boundingBox.Left;
                    if (boundingBox.Right > right)
                        right = boundingBox.Right;
                }

                return new OriginRectangle(left, top, right - left, bottom - top);
            }
        }

        public event Action<Collision> Colliding
        {
            add
            {
                foreach (ICollider collider in _colliderParent.Components)
                    collider.Colliding += value;
            }
            remove
            {
                foreach (ICollider collider in _colliderParent.Components)
                    collider.Colliding -= value;
            }
        }

        public event Action<Collision> Collided
        {
            add
            {
                foreach (ICollider collider in _colliderParent.Components)
                    collider.Collided += value;
            }
            remove
            {
                foreach (ICollider collider in _colliderParent.Components)
                    collider.Collided -= value;
            }
        }

        public void Initialize()
        {
            foreach (ICollider collider in _colliderParent.Components)
                collider.Initialize();
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            foreach (ICollider collider in _colliderParent.Components)
                collider.Update(elapsedTime);
        }

        public bool IsColliding(ICollider collider, out Collision collision)
        {
            if (IgnoredFilter != null && IgnoredFilter(collider))
            {
                collision = Collision.Empty;
                return false;
            }

            foreach (ICollider component in _colliderParent.Components)
                if (component.IsColliding(component, out collision))
                    return true;

            collision = new Collision();
            return false;
        }

        public bool Intersects(IRectangle rectangle)
        {
            return _colliderParent.Components.Any(component => component.Intersects(rectangle));
        }

        public bool Intersects(ICircle circle)
        {
            return _colliderParent.Components.Any(component => component.Intersects(circle));
        }

        public bool ContainsPoint(Vector2 point)
        {
            return _colliderParent.Components.Any(component => component.ContainsPoint(point));
        }
    }
}