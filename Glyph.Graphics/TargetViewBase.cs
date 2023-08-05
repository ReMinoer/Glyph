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
    public abstract class TargetViewBase : ViewBase, ILoadContent, IUpdate
    {
        private Quad _shape;
        protected readonly SceneNode _sceneNode;
        protected readonly ReadOnlySceneNode _readOnlySceneNode;
        protected readonly FillingRectangle _fillingRectangle;
        protected readonly FillingRenderer _fillingRenderer;

        public ViewEffectManager EffectManager { get; }
        protected override Quad Shape => _shape;
        public Texture2D Output => EffectManager.Texture;

        protected override float RenderDepth => SceneNode.Depth;
        protected override ISceneNode SceneNode => _readOnlySceneNode;

        public Vector2 Size
        {
            get => _fillingRectangle.Rectangle.Size;
            protected set
            {
                if (EffectManager.Size == value)
                    return;

                EffectManager.Size = value;
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
            Components.Add(EffectManager = new ViewEffectManager(graphicsDeviceFunc));
            Components.Add(_fillingRectangle = new FillingRectangle(_sceneNode));
            Components.Add(_fillingRenderer = new FillingRenderer(_fillingRectangle, EffectManager));

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
            EffectManager.Initialize();
        }

        public void LoadContent(IContentLibrary contentLibrary)
        {
            EffectManager.LoadContent(contentLibrary);
        }

        public async Task LoadContentAsync(IContentLibrary contentLibrary)
        {
            await EffectManager.LoadContentAsync(contentLibrary);
        }

        public void Update(ElapsedTime elapsedTime)
        {
            EffectManager.Update(elapsedTime);
        }

        public override void Draw(IDrawer drawer)
        {
            RenderTargetBinding[] renderTargetsBackup = drawer.GraphicsDevice.GetRenderTargets();

            var newDrawer = new Drawer(drawer, Camera.GetSceneNode().RootNode())
            {
                CurrentView = this
            };

            EffectManager.Prepare(newDrawer);
            EffectManager.CleanFirstRender(newDrawer.GraphicsDevice);

            var spriteBatchContext = new SpriteBatchContext
            {
                TransformMatrix = RenderMatrix
            };

            newDrawer.SpriteBatchStack.Push(spriteBatchContext);

            newDrawer.Render();

            EffectManager.Apply(newDrawer);
            newDrawer.SpriteBatchStack.Pop();

            newDrawer.GraphicsDevice.SetRenderTargets(renderTargetsBackup);

            _fillingRenderer.Draw(drawer);
        }
    }

    //public class RegionView : ViewBase
    //{
    //    public IView ParentView { get; set; }
    //    public TopLeftRectangle ViewportRectangle { get; set; }

    //    public TopLeftRectangle ScaledViewportRectangle
    //    {
    //        get => ViewportRectangle.NormalizedCoordinates(ParentView.BoundingBox);
    //        set => ViewportRectangle = value.Scale(ParentView.BoundingBox);
    //    }

    //    protected override IRectangle Shape => _sceneNode.Transform(new TopLeftRectangle(ViewportRectangle.Position - ParentView.Center, ViewportRectangle.Size));

    //    public override void Draw(IDrawer drawer)
    //    {
    //        if (!this.Displayed(drawer, drawer.Client, GetSceneNode()))
    //            return;

    //        Viewport previousViewport = drawer.GraphicsDevice.Viewport;

    //        var newDrawer = new Drawer(drawer.Client, drawer.Root, Camera.GetSceneNode().RootNode())
    //        {
    //            CurrentView = this
    //        };

    //        TopLeftRectangle viewportRectangle = ViewportRectangle;
    //        newDrawer.GraphicsDevice.Viewport = new Viewport
    //        {
    //            X = previousViewport.X + (int)viewportRectangle.Position.X,
    //            Y = previousViewport.Y + (int)viewportRectangle.Position.Y,
    //            Width = (int)System.Math.Ceiling(viewportRectangle.Width),
    //            Height = (int)System.Math.Ceiling(viewportRectangle.Height),
    //            MinDepth = 0,
    //            MaxDepth = 1
    //        };

    //        var spriteBatchContext = new SpriteBatchContext { SpriteSortMode = SpriteSortMode.BackToFront, TransformMatrix = RenderMatrix };

    //        newDrawer.SpriteBatchStack.Push(spriteBatchContext);
    //        newDrawer.Root?.Draw(newDrawer);
    //        newDrawer.SpriteBatchStack.Pop();

    //        newDrawer.GraphicsDevice.Viewport = previousViewport;
    //    }
    //}
}