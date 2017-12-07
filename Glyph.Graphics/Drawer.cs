using Glyph.Core;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class Drawer : IDrawer
    {
        public SpriteBatchStack SpriteBatchStack { get; }
        
        public IDrawClient Client { get; }
        public GraphicsDevice GraphicsDevice { get; }
        public RenderTarget2D DefaultRenderTarget { get; }
        public Matrix ResolutionMatrix { get; }

        public IView CurrentView { get; set; }
        public Texture2D Output => CurrentView.Output;
        public CenteredRectangle DisplayedRectangle => CurrentView.DisplayedRectangle;
        public Matrix ViewMatrix => CurrentView.Matrix;
        public TopLeftRectangle ScreenBounds => CurrentView.BoundingBox;

        public Drawer(IDrawClient drawClient)
        {
            Client = drawClient;
            GraphicsDevice = drawClient.GraphicsDevice;
            DefaultRenderTarget = drawClient.DefaultRenderTarget;
            ResolutionMatrix = drawClient.ResolutionMatrix;

            SpriteBatchStack = new SpriteBatchStack(new SpriteBatch(GraphicsDevice));
        }

        public void ApplyEffects(IDrawer drawer)
        {
            CurrentView.ApplyEffects(drawer);
        }
    }
}