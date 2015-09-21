using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Shapes
{
    public abstract class ShapedSpriteBase : GlyphComponent, ISpriteSource, ILoadContent
    {
        protected readonly GraphicsDevice GraphicsDevice;
        public Texture2D Texture { get; protected set; }
        public Color Color { get; set; }

        protected ShapedSpriteBase(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }

        public abstract void LoadContent(ContentLibrary contentLibrary);

        public override void Dispose()
        {
            if (Texture != null)
                Texture.Dispose();

            Texture = null;

            base.Dispose();
        }
    }
}