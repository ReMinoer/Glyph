using Glyph.Core;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class Drawer : IDrawer
    {
        public IDrawClient Client { get; }
        public GraphicsDevice GraphicsDevice { get; }
        public SpriteBatchStack SpriteBatchStack { get; }
        public Resolution Resolution { get; }
        public IView CurrentView { get; set; }
        public RenderTarget2D DefaultRenderTarget { get; }
        public Texture2D Output => CurrentView.Output;
        public CenteredRectangle DisplayedRectangle => CurrentView.DisplayedRectangle;
        public Matrix ViewMatrix => CurrentView.Matrix;
        public TopLeftRectangle ScreenBounds => CurrentView.BoundingBox;

        public Drawer(IDrawClient drawClient)
        {
            Client = drawClient;
            GraphicsDevice = drawClient.GraphicsDevice;
            Resolution = drawClient.Resolution;
            DefaultRenderTarget = drawClient.DefaultRenderTarget;
            SpriteBatchStack = new SpriteBatchStack(new SpriteBatch(GraphicsDevice));
        }

        public void ApplyEffects(IDrawer drawer)
        {
            CurrentView.ApplyEffects(drawer);
        }
    }
}