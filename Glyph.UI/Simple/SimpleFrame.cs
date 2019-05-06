using Glyph.Animation;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Graphics;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.UI.Simple
{
    public class SimpleFrame : GlyphObject, IFrame
    {
        private readonly SpriteTransformer _spriteTransformer;
        private readonly FillingRectangle _fillingRectangle;
        private Vector2 _size;
        public SceneNode SceneNode { get; private set; }
        public Motion Motion { get; private set; }
        public SimpleBorder Border { get; private set; }

        public Vector2 Size
        {
            get { return _size; }
            set
            {
                _size = value;
                Border.Size = value;

                _fillingRectangle.Rectangle = Bounds;
            }
        }

        public TopLeftRectangle Bounds
        {
            get { return new TopLeftRectangle(SceneNode.Position, Size); }
        }

        public Color Color
        {
            get { return _spriteTransformer.Color; }
            set { _spriteTransformer.Color = value; }
        }

        public SimpleFrame(GlyphResolveContext context)
            : base(context)
        {
            SceneNode = Add<SceneNode>();
            Motion = Add<Motion>();
            _spriteTransformer = Add<SpriteTransformer>();

            _fillingRectangle = Add<FillingRectangle>();
            _fillingRectangle.Rectangle = Bounds;
            Add<FilledRectangleSprite>();
            Add<FillingRenderer>();

            Border = Add<SimpleBorder>();

            SceneNode.Refreshed += node => _fillingRectangle.Rectangle = Bounds;
        }
    }
}