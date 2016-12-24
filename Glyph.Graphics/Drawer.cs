using Glyph.Core;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class Drawer : IDrawer
    {
        public GraphicsDevice GraphicsDevice { get; }
        public SpriteBatchStack SpriteBatchStack { get; }
        public Resolution Resolution { get; }
        public IView CurrentView { get; set; }
        public ICamera Camera => CurrentView.Camera;
        public RenderTarget2D DefaultRenderTarget { get; }
        public Texture2D Output => CurrentView.Output;
        public IRectangle DisplayedRectangle => CurrentView.DisplayedRectangle;
        public Matrix ViewMatrix => CurrentView.Matrix;
        public IRectangle ScreenBounds => CurrentView.BoundingBox;

        public Drawer(GraphicsDevice graphicsDevice, Resolution resolution, RenderTarget2D defaultRenderTarget = null)
        {
            GraphicsDevice = graphicsDevice;
            Resolution = resolution;
            DefaultRenderTarget = defaultRenderTarget;
            SpriteBatchStack = new SpriteBatchStack(new SpriteBatch(graphicsDevice));
        }

        public void ApplyEffects(IDrawer drawer)
        {
            CurrentView.ApplyEffects(drawer);
        }
    }
}