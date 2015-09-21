using Glyph.Animation;
using Glyph.Graphics.Shapes;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Physics.Colliders
{
    public class CircleCollider : ShapeColliderBase<Circle>
    {
        public CircleCollider(SceneNode sceneNode, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
            : base(new CircleSprite(graphicsDevice), sceneNode, spriteBatch)
        {
            Shape = new Circle(Vector2.Zero, 10);
        }

        public override void LoadContent(ContentLibrary contentLibrary)
        {
            base.LoadContent(contentLibrary);
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