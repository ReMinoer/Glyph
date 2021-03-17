using System.Collections.Generic;
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

        public DrawScheduler DrawScheduler { get; }
        public SpriteBatchStack SpriteBatchStack { get; }

        public IDrawClient Client { get; }
        public GraphicsDevice GraphicsDevice { get; }
        public RenderTarget2D DefaultRenderTarget { get; }

        public IView CurrentView { get; set; }
        public Quad DisplayedRectangle => CurrentView.DisplayedRectangle;
        public Matrix ViewMatrix => CurrentView.RenderMatrix;
        public Vector2 ViewSize => CurrentView.BoundingBox.Size;

        public Drawer(DrawScheduler drawScheduler, SpriteBatchStack spriteBatchStack, IDrawClient drawClient, params ISceneNode[] sceneRoots)
        {
            DrawScheduler = drawScheduler;

            Client = drawClient;
            _sceneRoots = sceneRoots;

            GraphicsDevice = drawClient.GraphicsDevice;
            DefaultRenderTarget = drawClient.DefaultRenderTarget;

            SpriteBatchStack = spriteBatchStack;
        }

        public bool DrawPredicate(ISceneNode sceneNode)
        {
            return _sceneRoots.Contains(sceneNode.RootNode());
        }

        public void Render()
        {
            IEnumerable<IRenderTask> schedule = CurrentView.RenderScheduler.Schedule;
            foreach (IRenderTask renderTask in schedule)
                renderTask.Render(this);
        }

        void IDrawer.RenderView()
        {
            IEnumerable<IDrawTask> schedule = DrawScheduler.GetSchedule(this);
            foreach (IDrawTask drawTask in schedule)
                drawTask.Draw(this);
        }
    }
}