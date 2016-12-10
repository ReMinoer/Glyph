using Glyph.Core;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class Drawer : IDrawer
    {
        public GraphicsDeviceManager GraphicsDeviceManager { get; }
        public SpriteBatchStack SpriteBatchStack { get; }
        public IView CurrentView { get; set; }
        public GraphicsDevice GraphicsDevice => GraphicsDeviceManager.GraphicsDevice;
        public ICamera Camera => CurrentView.Camera;
        public Texture2D Output => CurrentView.Output;
        public IRectangle DisplayedRectangle => CurrentView.DisplayedRectangle;
        public Matrix ViewMatrix => CurrentView.Matrix;
        public IRectangle ScreenBounds => CurrentView.BoundingBox;

        public Drawer(GraphicsDeviceManager graphicsDeviceManager)
        {
            GraphicsDeviceManager = graphicsDeviceManager;
            SpriteBatchStack = new SpriteBatchStack(new SpriteBatch(graphicsDeviceManager.GraphicsDevice));
        }

        public void ApplyEffects(IDrawer drawer)
        {
            CurrentView.ApplyEffects(drawer);
        }
    }
}