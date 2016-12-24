using System;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Shapes
{
    public abstract class ShapedSpriteBase : SpriteSourceBase, ILoadContent
    {
        protected Texture2D _texture;
        protected readonly Func<GraphicsDevice> GraphicsDeviceFunc;
        public Color Color { get; set; }
        public override event Action<ISpriteSource> Loaded;

        public override sealed Texture2D Texture
        {
            get { return _texture; }
        }

        protected ShapedSpriteBase(Func<GraphicsDevice> graphicsDeviceFunc)
        {
            GraphicsDeviceFunc = graphicsDeviceFunc;
            Color = Color.White;
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            GenerateTexture();

            if (Loaded != null)
                Loaded.Invoke(this);
        }

        protected abstract void GenerateTexture();

        public override void Dispose()
        {
            if (Texture != null)
                Texture.Dispose();

            _texture = null;

            base.Dispose();
        }
    }
}