using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Physics.Colliders
{
    public class RectangleCollider : ShapeColliderBase<IRectangle>
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

        public override IRectangle Shape
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
            return CollisionUtils.IsColliding(IntersectionUtils.RectangleWithRectangle, this, collider, out collision);
        }

        public override bool IsColliding(CircleCollider collider, out Collision collision)
        {
            return CollisionUtils.IsColliding(IntersectionUtils.RectangleWithCircle, this, collider, out collision);
        }

        public override bool IsColliding(IGridCollider collider, out Collision collision)
        {
            return CollisionUtils.IsShapeCollidingGrid(IntersectionUtils.RectangleWithRectangle, this, collider, out collision);
        }
    }
}