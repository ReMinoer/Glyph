using System.Linq;
using Glyph.Core.Colliders;
using Glyph.Space;

namespace Glyph.Core
{
    public class ColliderManager
    {
        private IWriteableSpace<ICollider> _space;

        public IWriteableSpace<ICollider> Space
        {
            get { return _space; }
            set
            {
                foreach (ICollider collider in _space)
                    value.Add(collider);

                _space.Clear();
                _space = value;
            }
        }

        public ColliderManager()
        {
            _space = new Space<ICollider>(x => x.BoundingBox);
        }

        internal void Add(ICollider collider)
        {
            Space.Add(collider);
        }

        internal void Remove(ICollider collider)
        {
            Space.Remove(collider);
        }

        internal bool ResolveOneCollision(ICollider collider, out Collision collision)
        {
            foreach (ICollider other in Space.GetAllItemsInRange(collider).Where(other => other != collider && other.Enabled && !other.Unphysical))
                if (collider.IsColliding(other, out collision))
                    return true;

            collision = Collision.Empty;
            return false;
        }
    }
}