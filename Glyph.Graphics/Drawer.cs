using Glyph.Core;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class Drawer : IDrawer
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        public SpriteBatchStack SpriteBatchStack { get; private set; }
        public IView CurrentView { get; set; }

        public GraphicsDevice GraphicsDevice
        {
            get { return _graphicsDeviceManager.GraphicsDevice; }
        }

        public ICamera Camera
        {
            get { return CurrentView.Camera; }
        }

        public Texture2D Output
        {
            get { return CurrentView.Output; }
        }

        public IRectangle DisplayedRectangle
        {
            get { return CurrentView.DisplayedRectangle; }
        }

        public Matrix ViewMatrix
        {
            get { return CurrentView.Matrix; }
        }

        public IRectangle ScreenBounds
        {
            get { return CurrentView.BoundingBox; }
        }

        public Drawer(SpriteBatch spriteBatch, GraphicsDeviceManager graphicsDeviceManager)
        {
            _graphicsDeviceManager = graphicsDeviceManager;
            SpriteBatchStack = new SpriteBatchStack(spriteBatch);
        }

        public void ApplyEffects(IDrawer drawer)
        {
            CurrentView.ApplyEffects(drawer);
        }
    }
}