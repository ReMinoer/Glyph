using System;
using System.Collections.Generic;
using Glyph.Composition;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using NLog;

namespace Glyph.Physics.Colliders
{
    public abstract class ColliderBase : GlyphContainer, ICollider
    {
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected readonly SceneNode SceneNode;
        private readonly ColliderManager _colliderManager;
        public bool Enabled { get; set; }
        public Predicate<ICollider> IgnoredFilter { get; set; }
        protected SceneNode ParentNode { get; private set; }

        public abstract IRectangle BoundingBox { get; }

        public Vector2 LocalCenter
        {
            get { return SceneNode.LocalPosition; }
            set { SceneNode.LocalPosition = value; }
        }

        public Vector2 Center
        {
            get { return SceneNode.Position; }
            set { SceneNode.Position = value; }
        }

        public event Action<Collision> Colliding;
        public event Action<Collision> Collided;

        protected ColliderBase(ColliderManager colliderManager)
        {
            Components.Add(SceneNode = new SceneNode());

            _colliderManager = colliderManager;
            _colliderManager.Add(this);

            Enabled = true;
            LocalCenter = Vector2.Zero;
        }

        public override void Initialize()
        {
            SceneNode.Initialize();

            for (ISceneNode sceneNode = SceneNode.ParentNode; sceneNode.ParentNode != null; sceneNode = sceneNode.ParentNode)
            {
                var writableSceneNode = sceneNode as SceneNode;
                if (writableSceneNode != null)
                {
                    ParentNode = writableSceneNode;
                    break;
                }
            }
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled || IsFreeze || Parent.IsFreeze)
                return;

            Collision collision;
            int count = 0;
            var alreadyResolved = new List<ICollider>();
            while (count < 5 && _colliderManager.ResolveOneCollision(this, out collision))
            {
                if (alreadyResolved.Contains(collision.OtherCollider))
                    break;

                if (Colliding != null)
                    Colliding.Invoke(collision);

                if (collision.IsHandled)
                    break;

                ParentNode.Position += collision.Correction;
                alreadyResolved.Add(collision.OtherCollider);

                if (Collided != null)
                    Collided.Invoke(collision);

                count++;
            }
        }

        public bool IsColliding(ICollider collider, out Collision collision)
        {
            if (IgnoredFilter != null && IgnoredFilter(collider))
            {
                collision = Collision.Empty;
                return false;
            }

            var rectangle = collider as RectangleCollider;
            if (rectangle != null)
                return IsColliding(rectangle, out collision);

            var circle = collider as CircleCollider;
            if (circle != null)
                return IsColliding(circle, out collision);

            var grid = collider as IGridCollider;
            if (grid != null)
                return IsColliding(grid, out collision);

            throw new NotSupportedException();
        }

        protected abstract bool IsColliding(RectangleCollider collider, out Collision collision);
        protected abstract bool IsColliding(CircleCollider collider, out Collision collision);
        protected abstract bool IsColliding(IGridCollider collider, out Collision collision);
        public abstract bool Intersects(IRectangle rectangle);
        public abstract bool Intersects(ICircle circle);
        public abstract bool ContainsPoint(Vector2 point);

        public override void Dispose()
        {
            _colliderManager.Remove(this);
            base.Dispose();
        }
    }
}