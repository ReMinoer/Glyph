using Diese.Injection;
using Glyph.Animation;
using Glyph.Composition;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    [SinglePerParent]
    public abstract class Renderer : GlyphComponent, IDraw
    {
        protected readonly ISpriteSource Source;
        protected readonly SceneNode SceneNode;
        public bool Visible { get; set; }

        [Injectable]
        public SpriteTransformer SpriteTransformer { get; set; }

        protected Renderer(ISpriteSource source, SceneNode sceneNode)
        {
            Source = source;
            SceneNode = sceneNode;

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