using System;
using Glyph.Composition;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Core.Colliders.Base
{
    public abstract class ColliderBase : GlyphContainer, ICollider
    {
        private const int CollisionDepth = 5;

        protected readonly SceneNode SceneNode;
        private readonly ColliderManager _colliderManager;

        public Predicate<ICollider> IgnoredFilter { get; set; }
        protected SceneNode ParentNode { get; private set; }
        public abstract TopLeftRectangle BoundingBox { get; }
        public abstract bool IsVoid { get; }
        public bool Unphysical { get; set; }

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

        bool ICollider.Active => Active;

        public event Action<Collision> Colliding;
        public event Action<Collision> Collided;

        protected ColliderBase(ColliderManager colliderManager)
        {
            Components.Add(SceneNode = new SceneNode());

            _colliderManager = colliderManager;
            _colliderManager.Add(this);

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
            if (!Enabled || Unphysical || IsFreeze || Parent.IsFreeze)
                return;

            int i = 0;
            while (i < CollisionDepth && _colliderManager.ResolveOneCollision(this, out Collision collision))
            {
                Colliding?.Invoke(collision);
                ParentNode.Position += collision.Correction;
                Collided?.Invoke(collision);
                i++;
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
        public abstract bool Intersects(Segment segment);
        public abstract bool Intersects<T>(T edgedShape) where T : IEdgedShape;
        public abstract bool Intersects(Circle circle);
        public abstract bool ContainsPoint(Vector2 point);

        public override void Dispose()
        {
            _colliderManager.Remove(this);
            base.Dispose();
        }
    }
}