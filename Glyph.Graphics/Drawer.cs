using System.Linq;
using Glyph.Core;
using Glyph.Math.Shapes;
using Glyph.Scheduling;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class Drawer : IDrawer
    {
        private readonly ISceneNode[] _sceneRoots;
        public SpriteBatchStack SpriteBatchStack { get; }
        
        public IDrawClient Client { get; }
        public GraphicsDevice GraphicsDevice { get; }
        public RenderTarget2D DefaultRenderTarget { get; }

        public IDrawTask Root { get; }

        public IView CurrentView { get; set; }
        public Quad DisplayedRectangle => CurrentView.DisplayedRectangle;
        public Matrix ViewMatrix => CurrentView.RenderMatrix;
        public Vector2 ViewSize => CurrentView.BoundingBox.Size;

        public Drawer(SpriteBatchStack spriteBatchStack, IDrawClient drawClient, IDrawTask root, params ISceneNode[] sceneRoots)
        {
            Client = drawClient;
            Root = root;
            _sceneRoots = sceneRoots;

            GraphicsDevice = drawClient.GraphicsDevice;
            DefaultRenderTarget = drawClient.DefaultRenderTarget;

            SpriteBatchStack = spriteBatchStack;
        }

        public bool DrawPredicate(ISceneNode sceneNode)
        {
            return _sceneRoots.Contains(sceneNode.RootNode());
        }
    }
}