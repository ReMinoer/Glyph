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
            var sceneNode = new SceneNode(parentNode);
            Components.Add(sceneNode);
            Components.Add(_shapedSprite = shapedSprite);
            Components.Add(SpriteTransformer = new SpriteTransformer());
            Components.Add(_spriteRenderer = new SpriteRenderer(_shapedSprite, sceneNode));

            _spriteRenderer.SpriteTransformer = SpriteTransformer;
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