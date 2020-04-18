using System;
using Glyph.Composition;
using Glyph.Core.Colliders.Base;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Core.Colliders
{
    public class ColliderComposite : GlyphComposite<ICollider>, ICollider
    {
        private readonly ColliderContainerImplementation _colliderContainerImplementation;

        public ColliderComposite()
        {
            _colliderContainerImplementation = new ColliderContainerImplementation(this);
        }

        public override bool Enabled
        {
            get => _colliderContainerImplementation.Enabled;
            set
            {
                base.Enabled = value;
                _colliderContainerImplementation.Enabled = value;
            }
        }

        public bool Unphysical
        {
            get => _colliderContainerImplementation.Unphysical;
            set => _colliderContainerImplementation.Unphysical = value;
        }

        public Vector2 Center
        {
            get => _colliderContainerImplementation.Center;
            set => _colliderContainerImplementation.Center = value;
        }

        public Vector2 LocalCenter
        {
            get => _colliderContainerImplementation.LocalCenter;
            set => _colliderContainerImplementation.LocalCenter = value;
        }

        public bool IsVoid => _colliderContainerImplementation.IsVoid;
        public TopLeftRectangle BoundingBox => _colliderContainerImplementation.BoundingBox;

        public Predicate<ICollider> IgnoredFilter
        {
            get => _colliderContainerImplementation.IgnoredFilter;
            set => _colliderContainerImplementation.IgnoredFilter = value;
        }

        public event Action<Collision> Colliding
        {
            add => _colliderContainerImplementation.Colliding += value;
            remove => _colliderContainerImplementation.Colliding -= value;
        }

        public event Action<Collision> Collided
        {
            add => _colliderContainerImplementation.Collided += value;
            remove => _colliderContainerImplementation.Collided -= value;
        }

        public override void Initialize()
        {
            base.Initialize();
            _colliderContainerImplementation.Initialize();
        }

        public void Update(ElapsedTime elapsedTime) => _colliderContainerImplementation.Update(elapsedTime);
        public bool ContainsPoint(Vector2 point) => _colliderContainerImplementation.ContainsPoint(point);
        public bool Intersects(Segment segment) => _colliderContainerImplementation.Intersects(segment);
        public bool Intersects<T>(T edgedShape) where T : IEdgedShape => _colliderContainerImplementation.Intersects(edgedShape);
        public bool Intersects(Circle circle) => _colliderContainerImplementation.Intersects(circle);
        public bool IsColliding(ICollider collider, out Collision collision) => _colliderContainerImplementation.IsColliding(collider, out collision);
    }
}