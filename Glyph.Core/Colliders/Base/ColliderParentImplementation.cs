using System;
using System.Linq;
using Glyph.Composition;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Core.Colliders.Base
{
    internal class ColliderParentImplementation : IMovableShape
    {
        private readonly IGlyphContainer<ICollider> _colliderContainer;
        public bool Enabled { get; set; }
        public bool Unphysical { get; set; }
        public Predicate<ICollider> IgnoredFilter { get; set; }
        public bool IsVoid => _colliderContainer.Components.All(x => x.IsVoid);

        public ColliderParentImplementation(IGlyphContainer<ICollider> colliderContainer)
        {
            _colliderContainer = colliderContainer;
        }

        public Vector2 Center
        {
            get
            {
                int i = 0;
                Vector2 center = Vector2.Zero;
                foreach (ICollider collider in _colliderContainer.Components)
                {
                    center += collider.Center;
                    i++;
                }
                return center / i;
            }
            set
            {
                Vector2 center = Center;
                foreach (ICollider collider in _colliderContainer.Components)
                    collider.Center = collider.Center - center + value;
            }
        }

        public Vector2 LocalCenter
        {
            get
            {
                int i = 0;
                Vector2 localCenter = Vector2.Zero;
                foreach (ICollider collider in _colliderContainer.Components)
                {
                    localCenter += collider.LocalCenter;
                    i++;
                }
                return localCenter / i;
            }
            set
            {
                Vector2 localCenter = LocalCenter;
                foreach (ICollider collider in _colliderContainer.Components)
                    collider.LocalCenter = collider.LocalCenter - localCenter + value;
            }
        }

        public TopLeftRectangle BoundingBox
        {
            get { return MathUtils.GetBoundingBox(_colliderContainer.Components); }
        }

        public event Action<Collision> Colliding
        {
            add
            {
                foreach (ICollider collider in _colliderContainer.Components)
                    collider.Colliding += value;
            }
            remove
            {
                foreach (ICollider collider in _colliderContainer.Components)
                    collider.Colliding -= value;
            }
        }

        public event Action<Collision> Collided
        {
            add
            {
                foreach (ICollider collider in _colliderContainer.Components)
                    collider.Collided += value;
            }
            remove
            {
                foreach (ICollider collider in _colliderContainer.Components)
                    collider.Collided -= value;
            }
        }

        public void Initialize()
        {
            foreach (ICollider collider in _colliderContainer.Components)
                collider.Initialize();
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            foreach (ICollider collider in _colliderContainer.Components)
                collider.Update(elapsedTime);
        }

        public bool IsColliding(ICollider collider, out Collision collision)
        {
            if (IgnoredFilter != null && IgnoredFilter(collider))
            {
                collision = Collision.Empty;
                return false;
            }

            foreach (ICollider component in _colliderContainer.Components)
                if (component.IsColliding(component, out collision))
                    return true;

            collision = new Collision();
            return false;
        }

        public bool Intersects(TopLeftRectangle rectangle)
        {
            return _colliderContainer.Components.Any(component => component.Intersects(rectangle));
        }

        public bool Intersects(Circle circle)
        {
            return _colliderContainer.Components.Any(component => component.Intersects(circle));
        }

        public bool ContainsPoint(Vector2 point)
        {
            return _colliderContainer.Components.Any(component => component.ContainsPoint(point));
        }
    }
}