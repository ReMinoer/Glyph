using System;
using Glyph.Composition;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Physics.Colliders
{
    public class ColliderComposite : GlyphComposite<ICollider>, ICollider
    {
        public bool Enabled { get; set; }
        public Predicate<ICollider> IgnoredFilter { get; set; }

        public Vector2 Center
        {
            get
            {
                Vector2 center = Vector2.Zero;
                foreach (ICollider collider in this)
                    center += collider.Center;
                return center / Components.Count;
            }
            set
            {
                Vector2 center = Center;
                foreach (ICollider collider in this)
                    collider.Center = collider.Center - center + value;
            }
        }
        public Vector2 LocalCenter
        {
            get
            {
                Vector2 localCenter = Vector2.Zero;
                foreach (ICollider collider in this)
                    localCenter += collider.LocalCenter;
                return localCenter / Components.Count;
            }
            set
            {
                Vector2 localCenter = LocalCenter;
                foreach (ICollider collider in this)
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

                foreach (ICollider collider in Components)
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
                foreach (ICollider collider in this)
                    collider.Colliding += value;
            }
            remove
            {
                foreach (ICollider collider in this)
                    collider.Colliding -= value;
            }
        }

        public event Action<Collision> Collided
        {
            add
            {
                foreach (ICollider collider in this)
                    collider.Collided += value;
            }
            remove
            {
                foreach (ICollider collider in this)
                    collider.Collided -= value;
            }
        }

        public override void Initialize()
        {
            foreach (ICollider collider in this)
                collider.Initialize();
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            foreach (ICollider collider in this)
                collider.Update(elapsedTime);
        }

        public bool IsColliding(ICollider collider, out Collision collision)
        {
            if (IgnoredFilter != null && IgnoredFilter(collider))
            {
                collision = Collision.Empty;
                return false;
            }

            foreach (ICollider component in this)
                if (component.IsColliding(component, out collision))
                    return true;

            collision = new Collision();
            return false;
        }

        public bool Intersects(IRectangle rectangle)
        {
            foreach (ICollider component in this)
                if (component.Intersects(rectangle))
                    return true;

            return false;
        }

        public bool Intersects(ICircle circle)
        {
            foreach (ICollider component in this)
                if (component.Intersects(circle))
                    return true;

            return false;
        }

        public bool ContainsPoint(Vector2 point)
        {
            foreach (ICollider component in this)
                if (component.ContainsPoint(point))
                    return true;

            return false;
        }
    }
}