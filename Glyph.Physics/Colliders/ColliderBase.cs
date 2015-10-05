using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Physics.Colliders
{
    public abstract class ColliderBase : GlyphComponent, ICollider
    {
        public bool Visible { get; set; }
        public abstract void LoadContent(ContentLibrary contentLibrary);
        public abstract bool Intersects(RectangleCollider collider);
        public abstract bool Intersects(CircleCollider collider);
        public abstract bool ContainsPoint(Vector2 point);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}