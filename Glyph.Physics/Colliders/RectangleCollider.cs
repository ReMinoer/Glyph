using System;
using Glyph.Animation;
using Glyph.Graphics.Shapes;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Physics.Colliders
{
    public class RectangleCollider : ShapeColliderBase<CenteredRectangle>
    {
        public RectangleCollider(SceneNode sceneNode, Lazy<SpriteBatch> lazySpriteBatch, Lazy<GraphicsDevice> lazyGraphicsDevice)
            : base(new FilledRectangleSprite(lazyGraphicsDevice), sceneNode, lazySpriteBatch)
        {
            Shape = new CenteredRectangle(Vector2.Zero, 10, 10);
        }

        public override void LoadContent(ContentLibrary contentLibrary)
        {
            base.LoadContent(contentLibrary);
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