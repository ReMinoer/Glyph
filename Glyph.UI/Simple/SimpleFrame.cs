using Diese.Injection;
using Glyph.Animation;
using Glyph.Composition;
using Glyph.Graphics;
using Glyph.Graphics.Shapes;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.UI.Simple
{
    public class SimpleFrame : GlyphObject, IFrame
    {
        private readonly SpriteTransformer _spriteTransformer;
        public SceneNode SceneNode { get; private set; }
        public Motion Motion { get; private set; }
        public Shadow? Shadow { get; set; }
        public SimpleBorder Border { get; private set; }

        public OriginRectangle Bounds
        {
            get { return Border.Bounds; }
        }

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
            _spriteTransformer = Add<SpriteTransformer>();

            Add<FilledRectangleSprite>();
            Add<FillingRenderer>();

            Border = Add<SimpleBorder>();

            SceneNode.Refreshed += node =>
            {
                Bounds.Center = SceneNode.Position;
                Border.Bounds.Center = SceneNode.Position;
            };
        }
    }
}