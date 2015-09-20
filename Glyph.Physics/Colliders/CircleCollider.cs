using Glyph.Animation;
using Glyph.Graphics;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Physics.Colliders
{
    public class CircleCollider : ShapeColliderBase<Circle>
    {
        private readonly TextureFactory _textureFactory;

        public CircleCollider(SceneNode sceneNode, SpriteBatch spriteBatch, TextureFactory textureFactory)
            : base(sceneNode, spriteBatch)
        {
            _textureFactory = textureFactory;

            Shape = new Circle(Vector2.Zero, 10);
        }

        public override void LoadContent(ContentLibrary contentLibrary)
        {
            SpriteTransformer.Texture = _textureFactory.CreateCircle(100, Color.Blue);
            SpriteTransformer.Scale = Vector2.One * Shape.Radius / 100;
        }

        public override bool Intersects(RectangleCollider collider)
        {
            return IntersectionUtils.RectangleAndCircle(collider.Shape, Shape);
        }

        public override bool Intersects(CircleCollider collider)
        {
            return IntersectionUtils.TwoCircles(Shape, collider.Shape);
        }
    }
}