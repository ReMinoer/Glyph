using System;
using System.Threading.Tasks;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Base;
using Glyph.Graphics.Renderer;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public abstract class TargetViewBase : ViewBase, ILoadContent
    {
        private Quad _shape;
        protected readonly SceneNode _sceneNode;
        protected readonly ReadOnlySceneNode _readOnlySceneNode;
        protected readonly FillingRectangle _fillingRectangle;
        protected readonly FillingRenderer _fillingRenderer;

        public PostProcessRenderer PostProcess { get; }

        public Texture2D Output => PostProcess.Texture;
        protected override sealed Quad Shape => _shape;
        protected override sealed float RenderDepth => SceneNode.Depth;
        protected override sealed ISceneNode SceneNode => _readOnlySceneNode;

        public Vector2 Size
        {
            get => _fillingRectangle.Rectangle.Size;
            protected set
            {
                if (PostProcess.Size == value)
                    return;

                PostProcess.Size = value;
                _fillingRectangle.Rectangle = new CenteredRectangle(Vector2.Zero, value);

                RefreshTransformation();
                SizeChanged?.Invoke(this, value);
            }
        }

        public override event EventHandler<Vector2> SizeChanged;
        public override event EventHandler RenderDepthChanged;

        protected TargetViewBase(Func<GraphicsDevice> graphicsDeviceFunc)
        {
            Components.Add(_sceneNode = new SceneNode());
            Components.Add(PostProcess = new PostProcessRenderer(graphicsDeviceFunc));
            Components.Add(_fillingRectangle = new FillingRectangle(_sceneNode));
            Components.Add(_fillingRenderer = new FillingRenderer(_fillingRectangle, PostProcess));

            DrawClientFilter = new ExcludingFilter<IDrawClient>();
            _fillingRenderer.DrawClientFilter = DrawClientFilter;

            _readOnlySceneNode = new ReadOnlySceneNode(_sceneNode);
            _sceneNode.Refreshed += OnSceneNodeRefreshed;
            _sceneNode.DepthChanged += OnSceneNodeDepthChanged;
        }
        
        private void OnSceneNodeRefreshed(SceneNodeBase obj) => RefreshTransformation();
        private void OnSceneNodeDepthChanged(object sender, EventArgs e) => RenderDepthChanged?.Invoke(this, EventArgs.Empty);

        protected override void RefreshTransformation()
        {
            _shape = _sceneNode.Transformation.Transform(_fillingRenderer.Shape);
            base.RefreshTransformation();
        }

        public override void Initialize()
        {
            _sceneNode.Initialize();
            PostProcess.Initialize();
        }

        public void LoadContent(IContentLibrary contentLibrary)
        {
            PostProcess.LoadContent(contentLibrary);
        }

        public async Task LoadContentAsync(IContentLibrary contentLibrary)
        {
            await PostProcess.LoadContentAsync(contentLibrary);
        }

        public override void Draw(IDrawer drawer)
        {
            RenderTargetBinding[] renderTargetsBackup = drawer.GraphicsDevice.GetRenderTargets();

            var newDrawer = new Drawer(drawer, Camera.GetSceneNode().RootNode())
            {
                CurrentView = this
            };
            
            PostProcess.CleanFirstRender(newDrawer.GraphicsDevice);

            var spriteBatchContext = new SpriteBatchContext
            {
                TransformMatrix = RenderMatrix
            };

            newDrawer.SpriteBatchStack.Push(spriteBatchContext);

            newDrawer.Render();

            PostProcess.Render(newDrawer);
            newDrawer.SpriteBatchStack.Pop();

            newDrawer.GraphicsDevice.SetRenderTargets(renderTargetsBackup);

            _fillingRenderer.Draw(drawer);
        }
    }
}