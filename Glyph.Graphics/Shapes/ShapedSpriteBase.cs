using System;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Shapes
{
    public abstract class ShapedSpriteBase : SpriteSourceBase, ILoadContent
    {
        protected readonly Lazy<GraphicsDevice> LazyGraphicsDevice;
        public Color Color { get; set; }

        protected ShapedSpriteBase(Lazy<GraphicsDevice> lazyGraphicsDevice)
        {
            LazyGraphicsDevice = lazyGraphicsDevice;
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