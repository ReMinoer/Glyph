using Glyph.Composition;
using Glyph.Composition.Injection;

namespace Glyph.Graphics.Renderer
{
    [SinglePerParent]
    public abstract class RendererBase : GlyphComponent, IDraw
    {
        public bool Visible { get; set; }
        public ISpriteSource Source { get; private set; }

        [GlyphInjectable(GlyphInjectableTargets.Fraternal)]
        public SpriteTransformer SpriteTransformer { get; set; }

        [GlyphInjectable(GlyphInjectableTargets.Parent)]
        public IDraw DrawableParent { get; set; }

        protected RendererBase(ISpriteSource source)
        {
            Source = source;
            Visible = true;
        }

        public void Draw(IDrawer drawer)
        {
            if (!Visible || (DrawableParent != null && !DrawableParent.Visible) || Source.Texture == null)
                return;

            Render(drawer);
        }

        protected abstract void Render(IDrawer drawer);
    }
}