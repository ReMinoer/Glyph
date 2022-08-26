using System;
using System.Threading.Tasks;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Shapes
{
    public abstract class ShapedSpriteBase : SpriteSourceBase, ILoadContent
    {
        private Texture2D _texture;
        protected readonly Func<GraphicsDevice> GraphicsDeviceFunc;
        public Color Color { get; set; }
        public override event Action<ISpriteSource> Loaded;

        public override sealed Texture2D Texture => _texture;

        protected ShapedSpriteBase(Func<GraphicsDevice> graphicsDeviceFunc)
        {
            GraphicsDeviceFunc = graphicsDeviceFunc;
            Color = Color.White;
        }

        public void LoadContent(IContentLibrary contentLibrary)
        {
            _texture = GenerateTexture();
            Loaded?.Invoke(this);
        }

        public async Task LoadContentAsync(IContentLibrary contentLibrary)
        {
            _texture = await Task.Run(GenerateTexture);
            Loaded?.Invoke(this);
        }

        protected abstract Texture2D GenerateTexture();

        public override void Dispose()
        {
            Texture?.Dispose();

            _texture = null;

            base.Dispose();
        }
    }
}