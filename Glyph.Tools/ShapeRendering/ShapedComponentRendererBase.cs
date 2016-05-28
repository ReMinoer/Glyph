using Glyph.Composition;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.ShapeRendering
{
    public abstract class ShapedComponentRendererBase : GlyphContainer, ILoadContent, IUpdate, IDraw
    {
        private readonly IShapedComponent _shapedComponent;
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

        protected ShapedComponentRendererBase(IShapedComponent shapedComponent, ShapedSpriteBase shapedSprite)
        {
            Visible = true;
            _shapedComponent = shapedComponent;

            Components.Add(_sceneNode = new SceneNode(shapedComponent.SceneNode));
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
            if (!Visible || _shapedComponent.GetComponentAmongParents(x => x is IEnableable && !((IEnableable)x).Enabled) != null)
                return;

            _spriteRenderer.Draw(drawer);
        }
    }
}