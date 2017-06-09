using System;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Graphics.Renderer;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class View : GlyphContainer, IView
    {
        private readonly SceneNode _sceneNode;
        private readonly FillingRectangle _fillingRectangle;
        private readonly FillingRenderer _fillingRenderer;
        public bool Visible { get; set; }
        public Predicate<IDrawer> DrawPredicate { get; set; }
        public IFilter<IDrawClient> DrawClientFilter { get; set; }
        public Camera Camera { get; set; }
        public ViewEffectManager EffectManager { get; private set; }
        public bool IsVoid => _fillingRectangle.Rectangle.IsVoid;

        public TopLeftRectangle BoundingBox
        {
            get { return _fillingRectangle.Rectangle; }
            set
            {
                EffectManager.Size = value.Size;
                _fillingRectangle.Rectangle = value;
            }
        }

        public CenteredRectangle DisplayedRectangle
        {
            get
            {
                return new CenteredRectangle
                {
                    Center = Camera.Position,
                    Size = BoundingBox.Size / Camera.Zoom
                };
            }
        }

        public Texture2D Output
        {
            get { return EffectManager.Texture; }
        }

        public ISceneNode SceneNode
        {
            get { return new ReadOnlySceneNode(_sceneNode); }
        }

        Camera IView.Camera
        {
            get { return Camera; }
        }

        public Matrix Matrix
        {
            get
            {
                return Matrix.CreateTranslation((-Camera.Position + (BoundingBox.Size / 2) / Camera.Zoom).ToVector3())
                    * Matrix.CreateRotationZ(-Camera.Rotation)
                    * Matrix.CreateScale(Camera.Zoom);
            }
        }

        public View(Func<GraphicsDevice> lazyGraphicsDevice)
        {
            Visible = true;

            Components.Add(_sceneNode = new SceneNode());
            Components.Add(EffectManager = new ViewEffectManager(lazyGraphicsDevice));
            Components.Add(_fillingRectangle = new FillingRectangle());
            Components.Add(_fillingRenderer = new FillingRenderer(_fillingRectangle, EffectManager, _sceneNode));
        }

        public override void Initialize()
        {
            _sceneNode.Initialize();
            EffectManager.Initialize();
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            EffectManager.LoadContent(contentLibrary);
        }

        public void Update(ElapsedTime elapsedTime)
        {
            EffectManager.Update(elapsedTime);
        }

        public void PrepareDraw(IDrawer drawer)
        {
            if (!Visible)
                return;

            EffectManager.Prepare(drawer);
            EffectManager.CleanFirstRender(drawer.GraphicsDevice);
            EffectManager.Apply(drawer);
        }

        public void ApplyEffects(IDrawer drawer)
        {
            if (!Visible)
                return;

            EffectManager.Apply(drawer);
        }

        public void Draw(IDrawer drawer)
        {
            if (!Visible)
                return;

            _fillingRenderer.Draw(drawer);
        }

        public bool IsVisibleOnView(Vector2 position)
        {
            return Visible && DisplayedRectangle.ContainsPoint(position);
        }

        public Vector2 ViewToScene(Vector2 viewPoint)
        {
            return (viewPoint - BoundingBox.Size / 2) / Camera.Zoom + Camera.Position;
        }

        public Vector2 SceneToView(Vector2 scenePosition)
        {
            return (scenePosition - Camera.Position) * Camera.Zoom + BoundingBox.Size / 2;
        }

        public bool ContainsPoint(Vector2 point)
        {
            return BoundingBox.ContainsPoint(point);
        }
    }
}