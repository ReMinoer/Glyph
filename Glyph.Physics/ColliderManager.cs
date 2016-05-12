using System.Collections.Generic;
using System.Linq;
using Glyph.Physics.Colliders;

namespace Glyph.Physics
{
    public class ColliderManager
    {
        private readonly List<ICollider> _colliders;

        public ColliderManager()
        {
            _colliders = new List<ICollider>();
        }

        internal void Add(ICollider collider)
        {
            _colliders.Add(collider);
        }

        internal void Remove(ICollider collider)
        {
            _colliders.Remove(collider);
        }

        internal bool ResolveOneCollision(ICollider collider, out Collision collision)
        {
            foreach (ICollider other in _colliders.Where(other => other != collider && other.Enabled))
                if (collider.IsColliding(other, out collision))
                    return true;

            collision = Collision.Empty;
            return false;
        }
    }
}