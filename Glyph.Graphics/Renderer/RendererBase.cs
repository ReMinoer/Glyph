using System;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Injection;

namespace Glyph.Graphics.Renderer
{
    [SinglePerParent]
    public abstract class RendererBase : GlyphComponent, IDraw
    {
        protected abstract float DepthProtected { get; }
        public bool Visible { get; set; }
        public Predicate<IDrawer> DrawPredicate { get; set; }
        public IFilter<IDrawClient> DrawClientFilter { get; set; }
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
            if (!this.Displayed(drawer.Client, drawer) || (DrawableParent != null && !DrawableParent.Visible) || !DepthManager.VisibilityPredicate(DepthProtected) || Source.Texture == null)
                return;

            Render(drawer);
        }

        protected abstract void Render(IDrawer drawer);
    }
}