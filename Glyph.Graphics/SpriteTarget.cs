using System;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class SpriteTarget : SpriteSourceBase, ILoadContent
    {
        private readonly Func<GraphicsDevice> _lazyGraphicsDevice;
        private Vector2 _size;
        private Texture2D _texture;
        public RenderTarget2D RenderTarget { get; private set; }
        public override event Action<ISpriteSource> Loaded;

        public override Texture2D Texture
        {
            get { return _texture; }
        }

        public Vector2 Size
        {
            get { return _size; }
            set
            {
                _size = value;
                RefreshRenderTarget();
            }
        }

        public SpriteTarget(Func<GraphicsDevice> lazyGraphicsDevice)
        {
            _lazyGraphicsDevice = lazyGraphicsDevice;
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            RefreshRenderTarget();
        }

        private void RefreshRenderTarget()
        {
            GraphicsDevice graphicsDevice = _lazyGraphicsDevice();
            if (graphicsDevice == null)
                return;

            RenderTarget = new RenderTarget2D(_lazyGraphicsDevice(), (int)Size.X, (int)Size.Y, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            _texture = RenderTarget;

            Loaded?.Invoke(this);
        }
    }
}