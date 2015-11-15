using Diese.Injection;
using Glyph.Animation;
using Glyph.Composition;
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

        [Injectable]
        public SpriteTransformer SpriteTransformer { get; set; }

        public SpriteArea(SceneNode sceneNode, ISpriteSource spriteSource)
        {
            _sceneNode = sceneNode;
            _spriteSource = spriteSource;
        }

        public bool ContainsPoint(Vector2 point)
        {
            Rectangle drawnRectangle = _spriteSource.GetDrawnRectangle();

            var rectangle = new CenteredRectangle
            {
                Center = _sceneNode.Position
            };

            if (SpriteTransformer != null)
                rectangle.Center += SpriteTransformer.Origin - drawnRectangle.Center.ToVector2();

            rectangle.Width = drawnRectangle.Width;
            rectangle.Height = drawnRectangle.Height;

            return rectangle.ContainsPoint(point);
        }
    }
}