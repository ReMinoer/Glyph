using Glyph.Core.Colliders;
using Glyph.Messaging;
using Microsoft.Xna.Framework;

namespace Glyph.Core
{
    public class Collision : HandlableMessage
    {
        static public Collision Empty => new Collision();

        public ICollider Sender { get; set; }
        public ICollider OtherCollider { get; set; }
        public Vector2 Correction { get; set; }
    }
}