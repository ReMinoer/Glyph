using Glyph.Core;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Base
{
    public abstract class SimpleHandleBase<TSpriteSource> : HandleBase
        where TSpriteSource : ISpriteSource
    {
        private readonly SpriteTransformer _spriteTransformer;
        protected readonly TSpriteSource _spriteSource;

        public Color Color
        {
            get => _spriteTransformer.Color;
            set => _spriteTransformer.Color = value;
        }

        protected SimpleHandleBase(GlyphResolveContext context, RootView rootView, ProjectionManager projectionManager)
            : base(context, rootView, projectionManager)
        {
            _spriteSource = Add<TSpriteSource>();
            _spriteTransformer = Add<SpriteTransformer>();
            Add<SpriteRenderer>();
        }
    }
}