using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Physics.Colliders
{
    public class RectangleCollider : ShapeColliderBase<CenteredRectangle>
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public Vector2 Size
        {
            get { return new Vector2(Width, Height); }
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        public override CenteredRectangle Shape
        {
            get { return new CenteredRectangle(Center, Width, Height); }
        }

        public RectangleCollider(Context context)
            : base(context)
        {
            Size = new Vector2(100, 100);
        }

        public override bool IsColliding(RectangleCollider collider, out Collision collision)
        {
            IRectangle intersection;
            if (IntersectionUtils.TwoRectangles(Shape, collider.Shape, out intersection))
            {
                Vector2 correction;

                bool isWiderThanTall = intersection.Width > intersection.Height;

                if (isWiderThanTall)
                {
                    correction = Shape.Top <= collider.Shape.Top
                        ? new Vector2(0, -intersection.Height)
                        : new Vector2(0, intersection.Height);
                }
                else
                {
                    correction = Shape.Right >= collider.Shape.Right
                        ? new Vector2(intersection.Width, 0)
                        : new Vector2(-intersection.Width, 0);
                }

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
            Vector2 correction;
            if (IntersectionUtils.RectangleAndCircle(Shape, (collider as ICollider<Circle>).Shape, out correction))
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