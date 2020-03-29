using Glyph.Core;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Tools.Transforming;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Base
{
    public abstract class SimpleHandleBase<TSpriteSource, TController> : HandleBase<TController>
        where TSpriteSource : ISpriteSource
        where TController : IAnchoredController
    {
        private readonly SpriteTransformer _spriteTransformer;
        protected readonly TSpriteSource _spriteSource;

        public Color Color
        {
            get => _spriteTransformer.Color;
            set => _spriteTransformer.Color = value;
        }

        protected SimpleHandleBase(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context, projectionManager)
        {
            _spriteSource = Add<TSpriteSource>();
            _spriteTransformer = Add<SpriteTransformer>();
            Add<SpriteRenderer>();
        }
    }
}