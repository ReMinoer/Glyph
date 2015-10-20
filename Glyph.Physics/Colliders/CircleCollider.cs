using System;
using Glyph.Graphics.Shapes;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Physics.Colliders
{
    public class CircleCollider : ShapeColliderBase<Circle>
    {
        public CircleCollider(Context context, Lazy<GraphicsDevice> lazyGraphicsDevice)
            : base(new CircleSprite(lazyGraphicsDevice), context)
        {
            Shape = new Circle(Vector2.Zero, 10);
        }

        public override void LoadContent(ContentLibrary contentLibrary)
        {
            base.LoadContent(contentLibrary);
            SpriteTransformer.Scale = Vector2.One * Shape.Radius / 100;
        }

        public override bool IsColliding(RectangleCollider collider, out Collision collision)
        {
            Vector2 correction;
            if (IntersectionUtils.RectangleAndCircle(collider.Shape, Shape, out correction))
            {
                collision = new Collision
                {
                    Sender = this,
                    OtherCollider = collider,
                    Correction = correction,
                    NewPosition = SceneNode.Position + correction
                };

                return true;
            }

            collision = new Collision();
            return false;
        }

        public override bool IsColliding(CircleCollider collider, out Collision collision)
        {
            float radiusIntersection;
            if (IntersectionUtils.TwoCircles(Shape, collider.Shape, out radiusIntersection))
            {
                Vector2 direction = (Shape.Center - collider.Shape.Center).Normalized();
                Vector2 correction = radiusIntersection * direction;

                collision = new Collision
                {
                    Sender = this,
                    OtherCollider = collider,
                    Correction = correction,
                    NewPosition = SceneNode.Position + correction
                };

                return true;
            }

            collision = new Collision();
            return false;
        }
    }
}