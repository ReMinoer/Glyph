using System.Linq;
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
        private readonly IBoxedComponent _boxedComponent;
        private readonly SceneNode _sceneNode;
        protected readonly SpriteTransformer SpriteTransformer;
        private readonly ShapedSpriteBase _shapedSprite;
        private readonly SpriteRenderer _spriteRenderer;
        public bool Visible { get; set; }

        public Color Color
        {
            get { return SpriteTransformer.Color; }
            set { SpriteTransformer.Color = value; }
        }

        protected ShapedComponentRendererBase(IBoxedComponent boxedComponent, ShapedSpriteBase shapedSprite)
        {
            Visible = true;
            _boxedComponent = boxedComponent;

            Components.Add(_sceneNode = new SceneNode(boxedComponent.SceneNode));
            Components.Add(_shapedSprite = shapedSprite);

            Components.Add(SpriteTransformer = new SpriteTransformer());
            SpriteTransformer.SpriteSource = _shapedSprite;

            Components.Add(_spriteRenderer = new SpriteRenderer(_shapedSprite, _sceneNode));

            _spriteRenderer.SpriteTransformer = SpriteTransformer;
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
            UpdateSize();
        }

        protected abstract void UpdateSize();

        public void Draw(IDrawer drawer)
        {
            if (!Visible || _boxedComponent.ParentQueue().OfType<IEnableable>().Any(x => !x.Enabled))
                return;

            _spriteRenderer.Draw(drawer);
        }
    }
}