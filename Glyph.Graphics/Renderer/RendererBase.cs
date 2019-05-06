using System;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Math;
using Glyph.Resolver;
using Niddle.Attributes;

namespace Glyph.Graphics.Renderer
{
    [SinglePerParent]
    public abstract class RendererBase : GlyphComponent, IDraw, IBoxedComponent
    {
        protected abstract float DepthProtected { get; }
        protected abstract ISceneNode SceneNode { get; }
        public bool Visible { get; set; }
        public Predicate<IDrawer> DrawPredicate { get; set; }
        public IFilter<IDrawClient> DrawClientFilter { get; set; }
        public ISpriteSource Source { get; private set; }
        public abstract IArea Area { get; }
        
        [Resolvable, ResolveTargets(ResolveTargets.Fraternal)]
        public SpriteTransformer SpriteTransformer { get; set; }
        
        [Resolvable, ResolveTargets(ResolveTargets.Parent)]
        public IDraw DrawableParent { get; set; }

        protected RendererBase(ISpriteSource source)
        {
            Source = source;
            Visible = true;
        }

        public void Draw(IDrawer drawer)
        {
            if (!this.Displayed(drawer, drawer.Client, SceneNode)
                || DrawableParent != null && !DrawableParent.Visible
                || !DepthManager.VisibilityPredicate(DepthProtected)
                || Source.Texture == null)
                return;

            Render(drawer);
        }

        protected abstract void Render(IDrawer drawer);
    }
}