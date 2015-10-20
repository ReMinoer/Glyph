using System;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Physics.Colliders
{
    public abstract class ColliderContainer : GlyphContainer<ICollider>, ICollider
    {
        public bool Enabled { get; set; }
        public bool Visible { get; set; }

        public Vector2 Center
        {
            get
            {
                Vector2 center = Vector2.Zero;
                foreach (ICollider collider in this)
                    center += collider.Center;
                return center / Components.Length;
            }
            set
            {
                Vector2 center = Center;
                foreach (ICollider collider in this)
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
                foreach (ICollider collider in this)
                    collider.Collided += value;
            }
            remove
            {
                foreach (ICollider collider in this)
                    collider.Collided -= value;
            }
        }

        protected ColliderContainer(int size)
            : base(size)
        {
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            foreach (ICollider collider in this)
                collider.LoadContent(contentLibrary);
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            foreach (ICollider collider in this)
                collider.Update(elapsedTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            foreach (ICollider collider in this)
                collider.Draw(spriteBatch);
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
    }
}