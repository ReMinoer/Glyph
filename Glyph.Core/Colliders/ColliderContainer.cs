using System;
using Glyph.Composition;
using Glyph.Core.Colliders.Base;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Core.Colliders
{
    public class ColliderContainer : GlyphContainer<ICollider>, ICollider
    {
        private readonly ColliderParentImplementation _colliderParentImplementation;

        public ColliderContainer()
        {
            _colliderParentImplementation = new ColliderParentImplementation(this);
        }

        public bool Enabled
        {
            get { return _colliderParentImplementation.Enabled; }
            set { _colliderParentImplementation.Enabled = value; }
        }

        public bool Unphysical
        {
            get { return _colliderParentImplementation.Unphysical; }
            set { _colliderParentImplementation.Unphysical = value; }
        }

        public Vector2 Center
        {
            get { return _colliderParentImplementation.Center; }
            set { _colliderParentImplementation.Center = value; }
        }

        public Vector2 LocalCenter
        {
            get { return _colliderParentImplementation.LocalCenter; }
            set { _colliderParentImplementation.LocalCenter = value; }
        }

        public bool IsVoid => _colliderParentImplementation.IsVoid;
        public TopLeftRectangle BoundingBox => _colliderParentImplementation.BoundingBox;

        public Predicate<ICollider> IgnoredFilter
        {
            get { return _colliderParentImplementation.IgnoredFilter; }
            set { _colliderParentImplementation.IgnoredFilter = value; }
        }

        public event Action<Collision> Colliding
        {
            add { _colliderParentImplementation.Colliding += value; }
            remove { _colliderParentImplementation.Colliding -= value; }
        }

        public event Action<Collision> Collided
        {
            add { _colliderParentImplementation.Collided += value; }
            remove { _colliderParentImplementation.Collided -= value; }
        }

        public override void Initialize()
        {
            base.Initialize();
            _colliderParentImplementation.Initialize();
        }

        public void Update(ElapsedTime elapsedTime)
        {
            _colliderParentImplementation.Update(elapsedTime);
        }

        public bool ContainsPoint(Vector2 point)
        {
            return _colliderParentImplementation.ContainsPoint(point);
        }

        public bool Intersects(TopLeftRectangle rectangle)
        {
            return _colliderParentImplementation.Intersects(rectangle);
        }

        public bool Intersects(Circle circle)
        {
            return _colliderParentImplementation.Intersects(circle);
        }

        public bool IsColliding(ICollider collider, out Collision collision)
        {
            return _colliderParentImplementation.IsColliding(collider, out collision);
        }
    }
}