using Diese.Injection;
using Glyph.Composition;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Renderer
{
    [SinglePerParent]
    public abstract class RendererBase : GlyphComponent, IDraw
    {
        public bool Visible { get; set; }
        public ISpriteSource Source { get; private set; }

        [Injectable]
        public SpriteTransformer SpriteTransformer { get; set; }
        
        protected RendererBase(ISpriteSource source)
        {
            Source = source;
            Visible = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible || Source.Texture == null)
                return;

            Render(spriteBatch);
        }

        protected abstract void Render(SpriteBatch spriteBatch);
    }
}