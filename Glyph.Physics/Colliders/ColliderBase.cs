using System;
using System.Collections.Generic;
using Glyph.Animation;
using Glyph.Composition;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Physics.Colliders
{
    public abstract class ColliderBase : GlyphComponent, ICollider
    {
        protected readonly SceneNode SceneNode;
        protected readonly SceneNode ParentNode;
        private readonly ColliderManager _colliderManager;
        public bool Enabled { get; set; }
        public bool Visible { get; set; }

        public Vector2 Center
        {
            get { return SceneNode.LocalPosition; }
            set { SceneNode.LocalPosition = value; }
        }

        public event Action<Collision> Collided;

        protected ColliderBase(Context context)
        {
            ParentNode = context.SceneNode;
            SceneNode = new SceneNode(ParentNode);

            _colliderManager = context.ColliderManager;
            _colliderManager.Add(this);

            Enabled = true;
            Center = Vector2.Zero;
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled || Parent.IsStatic)
                return;

            Collision collision;
            var alreadyResolved = new List<ICollider>();
            while (_colliderManager.ResolveOneCollision(this, out collision))
            {
                if (alreadyResolved.Contains(collision.OtherCollider))
                    break;

                if (Collided != null)
                    Collided.Invoke(collision);

                if (collision.IsCancelled)
                    continue;

                ParentNode.Position = collision.NewPosition;
                alreadyResolved.Add(collision.OtherCollider);
            }
        }

        public bool IsColliding(ICollider collider, out Collision collision)
        {
            var rectangle = collider as RectangleCollider;
            if (rectangle != null)
                return IsColliding(rectangle, out collision);

            var circle = collider as CircleCollider;
            if (circle != null)
                return IsColliding(circle, out collision);

            throw new NotSupportedException();
        }

        public abstract void LoadContent(ContentLibrary contentLibrary);
        public abstract bool IsColliding(RectangleCollider collider, out Collision collision);
        public abstract bool IsColliding(CircleCollider collider, out Collision collision);
        public abstract bool Intersects(IRectangle rectangle);
        public abstract bool Intersects(ICircle circle);
        public abstract bool ContainsPoint(Vector2 point);
        public abstract void Draw(SpriteBatch spriteBatch);

        public override void Dispose()
        {
            _colliderManager.Remove(this);
        }

        public class Context
        {
            public SceneNode SceneNode { get; set; }
            public ColliderManager ColliderManager { get; set; }

            public Context(SceneNode sceneNode, ColliderManager colliderManager)
            {
                SceneNode = sceneNode;
                ColliderManager = colliderManager;
            }
        }
    }
}