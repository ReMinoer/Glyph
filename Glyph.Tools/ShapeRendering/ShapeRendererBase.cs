using Glyph.Composition;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Tools.ShapeRendering
{
    public abstract class ShapeRendererBase : GlyphContainer, ILoadContent, IUpdate, IDraw
    {
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

        protected ShapeRendererBase(ISceneNode parentNode, ShapedSpriteBase shapedSprite)
        {
            Visible = true;
            
            Components.Add(_sceneNode = new SceneNode(parentNode));
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
            if (!Visible)
                return;

            _spriteRenderer.Draw(drawer);
        }
    }
}