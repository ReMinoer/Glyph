using Glyph.Core.Colliders.Base;
using Glyph.Math.Shapes;

namespace Glyph.Core.Colliders
{
    public class CircleCollider : ShapeColliderBase<Circle>
    {
        public float Radius { get; set; }

        public override Circle Shape
        {
            get { return new Circle(Center, Radius); }
        }

        public CircleCollider(ColliderManager colliderManager)
            : base(colliderManager)
        {
            Radius = 100;
        }

        protected override bool IsColliding(RectangleCollider collider, out Collision collision)
        {
            return CollisionUtils.IsColliding<Circle, TopLeftRectangle>(IntersectionUtils.CircleWithRectangle, this, collider, out collision);
        }

        protected override bool IsColliding(CircleCollider collider, out Collision collision)
        {
            return CollisionUtils.IsColliding(IntersectionUtils.CircleWithCircle, this, collider, out collision);
        }

        protected override bool IsColliding(IGridCollider collider, out Collision collision)
        {
            return CollisionUtils.IsShapeCollidingGrid(IntersectionUtils.CircleWithRectangle, this, collider, out collision);
        }
    }
}