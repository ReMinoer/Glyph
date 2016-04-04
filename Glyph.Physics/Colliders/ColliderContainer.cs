using System;
using Glyph.Composition;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Physics.Colliders
{
    public class ColliderComposite : GlyphComposite<ICollider>, ICollider
    {
        public bool Enabled { get; set; }

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

        public override bool IsStatic
        {
            get { return Parent.IsStatic; }
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

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            foreach (ICollider collider in this)
                collider.Update(elapsedTime);
        }

        public bool IsColliding(ICollider collider, out Collision collision)
        {
            foreach (ICollider component in this)
                if (component.IsColliding(component, out collision))
                    return true;

            collision = new Collision();
            return false;
        }

        public bool IsColliding(RectangleCollider collider, out Collision collision)
        {
            foreach (ICollider component in this)
                if (component.IsColliding(collider, out collision))
                    return true;

            collision = new Collision();
            return false;
        }

        public bool IsColliding(CircleCollider collider, out Collision collision)
        {
            foreach (ICollider component in this)
                if (component.IsColliding(collider, out collision))
                    return true;

            collision = new Collision();
            return false;
        }

        public bool IsColliding(IGridCollider collider, out Collision collision)
        {
            foreach (ICollider component in this)
                if (component.IsColliding(collider, out collision))
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