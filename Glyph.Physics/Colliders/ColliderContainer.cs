using System;
using Glyph.Composition;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Physics.Colliders
{
    public class ColliderComposite : GlyphComposite, ICollider
    {
        public bool Enabled { get; set; }

        public Vector2 Center
        {
            get
            {
                Vector2 center = Vector2.Zero;
                foreach (ICollider collider in GetAllComponents<ICollider>())
                    center += collider.Center;
                return center / Components.Count;
            }
            set
            {
                Vector2 center = Center;
                foreach (ICollider collider in GetAllComponents<ICollider>())
                    collider.Center = collider.Center - center + value;
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
                foreach (ICollider collider in GetAllComponents<ICollider>())
                    collider.Collided += value;
            }
            remove
            {
                foreach (ICollider collider in GetAllComponents<ICollider>())
                    collider.Collided -= value;
            }
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            foreach (ICollider collider in GetAllComponents<ICollider>())
                collider.Update(elapsedTime);
        }

        public bool IsColliding(ICollider collider, out Collision collision)
        {
            foreach (ICollider component in GetAllComponents<ICollider>())
                if (component.IsColliding(component, out collision))
                    return true;

            collision = new Collision();
            return false;
        }

        public bool IsColliding(RectangleCollider collider, out Collision collision)
        {
            foreach (ICollider component in GetAllComponents<ICollider>())
                if (component.IsColliding(collider, out collision))
                    return true;

            collision = new Collision();
            return false;
        }

        public bool IsColliding(CircleCollider collider, out Collision collision)
        {
            foreach (ICollider component in GetAllComponents<ICollider>())
                if (component.IsColliding(collider, out collision))
                    return true;

            collision = new Collision();
            return false;
        }

        public bool Intersects(IRectangle rectangle)
        {
            foreach (ICollider component in GetAllComponents<ICollider>())
                if (component.Intersects(rectangle))
                    return true;

            return false;
        }

        public bool Intersects(ICircle circle)
        {
            foreach (ICollider component in GetAllComponents<ICollider>())
                if (component.Intersects(circle))
                    return true;

            return false;
        }

        public bool ContainsPoint(Vector2 point)
        {
            foreach (ICollider component in GetAllComponents<ICollider>())
                if (component.ContainsPoint(point))
                    return true;

            return false;
        }
    }
}