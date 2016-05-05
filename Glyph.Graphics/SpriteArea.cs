using Diese.Injection;
using Glyph.Composition;
using Glyph.Composition.Injection;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Graphics
{
    // TODO : Handle sprite orientation & scale in SpriteArea
    public class SpriteArea : GlyphComponent, IArea
    {
        private readonly SceneNode _sceneNode;
        private readonly ISpriteSource _spriteSource;

        [GlyphInjectable(GlyphInjectableTargets.Fraternal)]
        public SpriteTransformer SpriteTransformer { get; set; }

        public SpriteArea(SceneNode sceneNode, ISpriteSource spriteSource)
        {
            _sceneNode = sceneNode;
            _spriteSource = spriteSource;
        }

        public IRectangle BoundingBox
        {
            get
            {
                Rectangle drawnRectangle = _spriteSource.GetDrawnRectangle();

                var rectangle = new CenteredRectangle
                {
                    Center = _sceneNode.Position,
                    Width = drawnRectangle.Width,
                    Height = drawnRectangle.Height
                };

                if (SpriteTransformer != null)
                    rectangle.Center += SpriteTransformer.Origin - drawnRectangle.Center.ToVector2();

                return rectangle;
            }
        }

        public bool ContainsPoint(Vector2 point)
        {
            return BoundingBox.ContainsPoint(point);
        }
    }
}