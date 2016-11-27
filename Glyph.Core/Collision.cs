using Glyph.Core.Colliders;
using Microsoft.Xna.Framework;

namespace Glyph.Core
{
    public struct Collision : IHandlable
    {
        public ICollider Sender { get; set; }
        public ICollider OtherCollider { get; set; }
        public Vector2 Correction { get; set; }
        public bool IsHandled { get; private set; }

        static public Collision Empty
        {
            get { return new Collision(); }
        }

        public void Handle()
        {
            IsHandled = true;
        }
    }
}