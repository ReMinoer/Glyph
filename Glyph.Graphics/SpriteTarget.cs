using System;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class SpriteTarget : SpriteSourceBase, ILoadContent
    {
        private readonly Lazy<GraphicsDevice> _lazyGraphicsDevice;
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

        public SpriteTarget(Lazy<GraphicsDevice> lazyGraphicsDevice)
        {
            _lazyGraphicsDevice = lazyGraphicsDevice;
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            RefreshRenderTarget();
        }

        private void RefreshRenderTarget()
        {
            if (!_lazyGraphicsDevice.IsValueCreated)
                return;

            RenderTarget = new RenderTarget2D(_lazyGraphicsDevice.Value, (int)Size.X, (int)Size.Y, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            _texture = RenderTarget;

            if (Loaded != null)
                Loaded.Invoke(this);
        }
    }
}