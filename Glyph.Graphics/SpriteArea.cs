using Glyph.Composition;
using Glyph.Core;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Resolver;
using Microsoft.Xna.Framework;
using Niddle.Attributes;

namespace Glyph.Graphics
{
    // TODO : Handle sprite orientation & scale in SpriteArea
    public class SpriteArea : GlyphComponent, IArea
    {
        private readonly SceneNode _sceneNode;
        private readonly ISpriteSource _spriteSource;
        
        [Resolvable, ResolveTargets(ResolveTargets.Fraternal)]
        public SpriteTransformer SpriteTransformer { get; set; }

        public SpriteArea(SceneNode sceneNode, ISpriteSource spriteSource)
        {
            _sceneNode = sceneNode;
            _spriteSource = spriteSource;
        }

        public bool IsVoid
        {
            get
            {
                Rectangle drawnRectangle = _spriteSource.GetDrawnRectagle();
                return drawnRectangle.Width == 0 && drawnRectangle.Height == 0;
            }
        }

        public TopLeftRectangle BoundingBox
        {
            get
            {
                Vector2 defaultOrigin = _spriteSource.GetDefaultOrigin();

                var rectangle = new CenteredRectangle
                {
                    Center = _sceneNode.Position,
                    Width = defaultOrigin.X,
                    Height = defaultOrigin.Y
                };

                if (SpriteTransformer != null)
                    rectangle.Center += SpriteTransformer.Origin - defaultOrigin / 2;

                return rectangle;
            }
        }

        public bool ContainsPoint(Vector2 point) => BoundingBox.ContainsPoint(point);
        public bool Intersects(Segment segment) => BoundingBox.Intersects(segment);
        public bool Intersects<T>(T edgedShape) where T : IEdgedShape => BoundingBox.Intersects(edgedShape);
        public bool Intersects(Circle circle) => BoundingBox.Intersects(circle);
    }
}