using System;
using System.Linq;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Microsoft.Xna.Framework;
using Stave;

namespace Glyph.Tools.ShapeRendering
{
    public abstract class ShapedComponentRendererBase : GlyphContainer, ILoadContent, IUpdate, IDraw
    {
        protected readonly FillingRectangle FillingRectangle;
        private readonly IBoxedComponent _boxedComponent;
        private readonly SceneNode _sceneNode;
        private readonly SpriteTransformer _spriteTransformer;
        private readonly ShapedSpriteBase _shapedSprite;
        private readonly FillingRenderer _fillingRenderer;
        public bool Visible { get; set; }
        public Predicate<IDrawer> DrawPredicate { get; set; }
        public IFilter<IDrawClient> DrawClientFilter { get; set; }

        public Color Color
        {
            get => _spriteTransformer.Color;
            set => _spriteTransformer.Color = value;
        }

        protected ShapedComponentRendererBase(IBoxedComponent boxedComponent, ShapedSpriteBase shapedSprite)
        {
            Visible = true;
            _boxedComponent = boxedComponent;

            Components.Add(_sceneNode = new SceneNode());
            Components.Add(FillingRectangle = new FillingRectangle());
            Components.Add(_shapedSprite = shapedSprite);

            Components.Add(_spriteTransformer = new SpriteTransformer());
            _spriteTransformer.SpriteSource = _shapedSprite;
            _spriteTransformer.Origin = Vector2.Zero;

            Components.Add(_fillingRenderer = new FillingRenderer(FillingRectangle, _shapedSprite, _sceneNode));
            _fillingRenderer.SpriteTransformer = _spriteTransformer;
        }

        public override void Initialize()
        {
            _sceneNode.Initialize();
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            _shapedSprite.LoadContent(contentLibrary);
        }

        public void Update(ElapsedTime elapsedTime)
        {
            FillingRectangle.Rectangle = _boxedComponent.Area.BoundingBox;
        }

        public void Draw(IDrawer drawer)
        {
            if (!this.Displayed(drawer.Client, drawer) || _boxedComponent.ParentQueue().OfType<IEnableable>().Any(x => !x.Enabled))
                return;

            _fillingRenderer.Draw(drawer);
        }
    }
}