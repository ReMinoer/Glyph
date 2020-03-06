using System;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Math;
using Glyph.Resolver;
using Niddle.Attributes;

namespace Glyph.Graphics.Renderer.Base
{
    public abstract class RendererBase : GlyphComponent, IDraw, IBoxedComponent
    {
        protected abstract float DepthProtected { get; }
        protected abstract ISceneNode SceneNode { get; }

        public bool Visible { get; set; } = true;
        public Predicate<IDrawer> DrawPredicate { get; set; }
        public IFilter<IDrawClient> DrawClientFilter { get; set; }

        public abstract IArea Area { get; }
        
        [Resolvable, ResolveTargets(ResolveTargets.Parent)]
        public IDraw DrawableParent { get; set; }

        public virtual void Draw(IDrawer drawer)
        {
            if (!this.Displayed(drawer, drawer.Client, SceneNode)
                || DrawableParent != null && !DrawableParent.Visible
                || !DepthManager.VisibilityPredicate(DepthProtected))
                return;

            Render(drawer);
        }

        protected abstract void Render(IDrawer drawer);
    }
}