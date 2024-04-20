using System;
using System.Threading.Tasks;
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

        public override Texture2D Texture => _texture;

        public Vector2 Size
        {
            get => _size;
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

        public void LoadContent(IContentLibrary contentLibrary)
        {
            RefreshRenderTarget();
        }

        public Task LoadContentAsync(IContentLibrary contentLibrary)
        {
            LoadContent(contentLibrary);
            return Task.CompletedTask;
        }

        private void RefreshRenderTarget()
        {
            if (Size.X <= 0 || Size.Y <= 0)
            {
                RenderTarget = null;
                _texture = null;
                Loaded?.Invoke(this);
                return;
            }

            GraphicsDevice graphicsDevice = _lazyGraphicsDevice();
            if (graphicsDevice == null)
            {
                RenderTarget = null;
                _texture = null;
                Loaded?.Invoke(this);
                return;
            }

            RenderTarget = new RenderTarget2D(_lazyGraphicsDevice(), (int)Size.X, (int)Size.Y, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8)
            {
                Name = Id.ToString()
            };

            _texture = RenderTarget;
            Loaded?.Invoke(this);
        }
    }
}