using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public class Drawer : IDrawer
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        public SpriteBatchStack SpriteBatchStack { get; private set; }

        public GraphicsDevice GraphicsDevice
        {
            get { return _graphicsDeviceManager.GraphicsDevice; }
        }

        public Drawer(SpriteBatch spriteBatch, GraphicsDeviceManager graphicsDeviceManager)
        {
            _graphicsDeviceManager = graphicsDeviceManager;
            SpriteBatchStack = new SpriteBatchStack(spriteBatch);
        }
    }
}