using System.Linq;
using Glyph.Physics.Colliders;
using Glyph.Space;

namespace Glyph.Physics
{
    public class ColliderManager
    {
        private ISpace<ICollider> _space;

        public ISpace<ICollider> Space
        {
            get { return _space; }
            set
            {
                foreach (ICollider collider in _space)
                    value.Add(collider);
                
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
            foreach (ICollider other in Space.GetAllItemsInRange(collider).Where(other => other != collider && other.Enabled))
                if (collider.IsColliding(other, out collision))
                    return true;

            collision = Collision.Empty;
            return false;
        }
    }
}