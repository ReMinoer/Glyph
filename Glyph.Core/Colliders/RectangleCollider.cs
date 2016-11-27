using Glyph.Core.Colliders.Base;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Core.Colliders
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

        public RectangleCollider(ColliderManager colliderManager)
            : base(colliderManager)
        {
            Size = new Vector2(100, 100);
        }

        protected override bool IsColliding(RectangleCollider collider, out Collision collision)
        {
            return CollisionUtils.IsColliding(IntersectionUtils.RectangleWithRectangle, this, collider, out collision);
        }

        protected override bool IsColliding(CircleCollider collider, out Collision collision)
        {
            return CollisionUtils.IsColliding(IntersectionUtils.RectangleWithCircle, this, collider, out collision);
        }

        protected override bool IsColliding(IGridCollider collider, out Collision collision)
        {
            return CollisionUtils.IsShapeCollidingGrid(IntersectionUtils.RectangleWithRectangle, this, collider, out collision);
        }
    }
}