using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Physics.Colliders
{
    public class CircleCollider : ShapeColliderBase<Circle>
    {
        public float Radius { get; set; }

        public override Circle Shape
        {
            get { return new Circle(Center, Radius); }
        }

        public CircleCollider(Context context)
            : base(context)
        {
            Radius = 100;
        }

        public override bool IsColliding(RectangleCollider collider, out Collision collision)
        {
            Vector2 correction;
            if (IntersectionUtils.RectangleAndCircle((collider as ICollider<CenteredRectangle>).Shape, Shape, out correction))
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

        public override bool Intersects(IRectangle rectangle)
        {
            return Shape.Intersects(rectangle);
        }

        public override bool Intersects(ICircle circle)
        {
            return Shape.Intersects(circle);
        }
    }
}