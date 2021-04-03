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
        static private readonly Matrix ViewEffectMatrix = Matrix.CreateLookAt(Vector3.Backward, Vector3.Zero, Vector3.Up);

        private readonly ISceneNode[] _sceneRoots;
        private readonly Matrix _spriteBatchOrthographicMatrix;
        private Matrix _projectionMatrix;
        
        public DrawScheduler DrawScheduler { get; }
        public SpriteBatchStack SpriteBatchStack { get; }

        public IDrawClient Client { get; }
        public GraphicsDevice GraphicsDevice { get; }
        public RenderTarget2D DefaultRenderTarget { get; }

        private IView _currentView;
        public IView CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;

                DisplayedRectangle = CurrentView.DisplayedRectangle;
                ViewSize = CurrentView.BoundingBox.Size;

                ViewMatrix = CurrentView.RenderMatrix;
                SpriteBatchMatrix = CurrentView.RenderMatrix * _spriteBatchOrthographicMatrix;
                
                _projectionMatrix = Matrix.CreateOrthographicOffCenter(
                    DisplayedRectangle.Left, DisplayedRectangle.Right, DisplayedRectangle.Bottom, DisplayedRectangle.Top,
                    float.MinValue / 2, float.MaxValue / 2);
            }
        }

        public Quad DisplayedRectangle { get; private set; }
        public Vector2 ViewSize { get; private set; }
        public Matrix ViewMatrix { get; private set; }
        public Matrix SpriteBatchMatrix { get; private set; }

        public Drawer(DrawScheduler drawScheduler, SpriteBatchStack spriteBatchStack, IDrawClient drawClient, params ISceneNode[] sceneRoots)
        {
            DrawScheduler = drawScheduler;

            Client = drawClient;
            _sceneRoots = sceneRoots;

            GraphicsDevice = drawClient.GraphicsDevice;
            DefaultRenderTarget = drawClient.DefaultRenderTarget;

            SpriteBatchStack = spriteBatchStack;

            _spriteBatchOrthographicMatrix = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, -1);;
        }

        public Drawer(IDrawer drawer, params ISceneNode[] sceneRoots)
            : this(drawer.DrawScheduler, drawer.SpriteBatchStack, drawer.Client, sceneRoots)
        {
        }

        public Matrix GetWorldViewProjectionMatrix(ISceneNode sceneNode)
        {
            return sceneNode.Matrix.ToMatrix4X4(sceneNode.Depth) * ViewEffectMatrix * _projectionMatrix;
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