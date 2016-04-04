using Glyph.Math.Shapes;

namespace Glyph.Physics.Colliders
{
    public class CircleCollider : ShapeColliderBase<ICircle>
    {
        public float Radius { get; set; }

        public override ICircle Shape
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
            return CollisionUtils.IsColliding(IntersectionUtils.CircleWithRectangle, this, collider, out collision);
        }

        public override bool IsColliding(CircleCollider collider, out Collision collision)
        {
            return CollisionUtils.IsColliding(IntersectionUtils.CircleWithCircle, this, collider, out collision);
        }

        public override bool IsColliding(IGridCollider collider, out Collision collision)
        {
            return CollisionUtils.IsShapeCollidingGrid(IntersectionUtils.CircleWithRectangle, this, collider, out collision);
        }
    }
}