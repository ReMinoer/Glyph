using Glyph.Composition;
using Glyph.Resolver;
using Niddle.Attributes;

namespace Glyph.Graphics.Renderer.Base
{
    [SinglePerParent]
    public abstract class SpriteRendererBase : RendererBase
    {
        public ISpriteSource Source { get; }
        
        [Resolvable, ResolveTargets(ResolveTargets.Fraternal)]
        public SpriteTransformer SpriteTransformer { get; set; }

        protected SpriteRendererBase(ISpriteSource source)
        {
            Source = source;
        }

        public override void Draw(IDrawer drawer)
        {
            if (Source.Texture == null)
                return;

            base.Draw(drawer);
        }
    }
}