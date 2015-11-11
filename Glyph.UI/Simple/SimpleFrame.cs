using Diese.Injection;
using Glyph.Animation;
using Glyph.Composition;
using Glyph.Graphics;
using Glyph.Graphics.Shapes;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.UI.Simple
{
    public class SimpleFrame : GlyphObject, IFrame, ILoadContent
    {
        private readonly FilledRectangleSprite _filledRectangleSprite;
        private readonly FillingRenderer _fillingRenderer;
        private readonly SpriteTransformer _spriteTransformer;
        public SceneNode SceneNode { get; private set; }
        public Motion Motion { get; private set; }
        public Shadow? Shadow { get; set; }
        public SimpleBorder Border { get; private set; }
        public IRectangle Bounds { get; set; }

        public Color Color
        {
            get { return _spriteTransformer.Color; }
            set { _spriteTransformer.Color = value; }
        }

        public SimpleFrame(IDependencyInjector injector)
            : base(injector)
        {
            SceneNode = Add<SceneNode>();
            Motion = Add<Motion>();
            Border = Add<SimpleBorder>();
            _spriteTransformer = Add<SpriteTransformer>();
            _filledRectangleSprite = Add<FilledRectangleSprite>();
            _fillingRenderer = Add<FillingRenderer>();

            SceneNode.Refreshed += node =>
            {
                Bounds.Center = SceneNode.Position;
                Border.Bounds.Center = SceneNode.Position;
            };
        }
    }
}