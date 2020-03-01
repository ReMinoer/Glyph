using Glyph.Core;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Tools.Base;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming.Base
{
    public abstract class AdvancedHandleBase : HandleBase
    {
        private FilledRectangleSprite _rectangleSprite;
        private readonly SpriteTransformer _spriteTransformer;

        protected override IArea Area => new CenteredRectangle(_sceneNode.Position, Size);

        private Vector2 _size;
        public Vector2 Size
        {
            get => _size;
            set
            {
                _size = value;
                _spriteTransformer.Scale = value.Divide(_rectangleSprite.Size.X, _rectangleSprite.Size.Y);
            }
        }

        public Color Color
        {
            get => _spriteTransformer.Color;
            set => _spriteTransformer.Color = value;
        }

        public AdvancedHandleBase(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context, projectionManager)
        {
            _rectangleSprite = Add<FilledRectangleSprite>();
            _spriteTransformer = Add<SpriteTransformer>();
            Add<SpriteRenderer>();
        }
    }
}