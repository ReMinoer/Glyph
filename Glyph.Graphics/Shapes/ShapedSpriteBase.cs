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
        public override event Action<ISpriteSource> Loaded;

        protected ShapedSpriteBase(Lazy<GraphicsDevice> lazyGraphicsDevice)
        {
            LazyGraphicsDevice = lazyGraphicsDevice;
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            GenerateTexture();

            if (Loaded != null)
                Loaded.Invoke(this);
        }

        public abstract void GenerateTexture();

        public override void Dispose()
        {
            if (Texture != null)
                Texture.Dispose();

            Texture = null;

            base.Dispose();
        }
    }
}