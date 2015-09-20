using Glyph.Animation;
using Glyph.Graphics;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Physics.Colliders
{
    public class RectangleCollider : ShapeColliderBase<CenteredRectangle>
    {
        private readonly TextureFactory _textureFactory;

        public RectangleCollider(SceneNode sceneNode, SpriteBatch spriteBatch, TextureFactory textureFactory)
            : base(sceneNode, spriteBatch)
        {
            _textureFactory = textureFactory;

            Shape = new CenteredRectangle(Vector2.Zero, 10, 10);
        }

        public override void LoadContent(ContentLibrary contentLibrary)
        {
            SpriteTransformer.Texture = _textureFactory.CreateFilledRectangle(1, 1, Color.Blue);
            SpriteTransformer.Scale = Shape.Size;
        }

        public override bool Intersects(RectangleCollider collider)
        {
            return IntersectionUtils.TwoRectangles(Shape, collider.Shape);
        }

        public override bool Intersects(CircleCollider collider)
        {
            return IntersectionUtils.RectangleAndCircle(Shape, collider.Shape);
        }
    }
}