using System;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Space;
using Microsoft.Xna.Framework;

namespace Glyph.Physics.Colliders
{
    public class GridCollider<TData> : ColliderBase, IGridCollider
    {
        public IGrid<TData> Grid { get; set; }

        public Func<TData, bool> IsCollidableCaseDelegate { get; set; }

        IGrid IGridCollider.Grid
        {
            get { return Grid; }
        }

        public override IRectangle BoundingBox
        {
            get { return Grid.BoundingBox; }
        }

        public GridCollider(Context context)
            : base(context)
        {
        }

        public override bool IsColliding(RectangleCollider collider, out Collision collision)
        {
            return CollisionUtils.IsGridCollidingShape(IntersectionUtils.RectangleWithRectangle, this, collider, out collision);
        }

        public override bool IsColliding(CircleCollider collider, out Collision collision)
        {
            return CollisionUtils.IsGridCollidingShape(IntersectionUtils.RectangleWithCircle, this, collider, out collision);
        }

        public override bool IsColliding(IGridCollider collider, out Collision collision)
        {
            throw new NotImplementedException();
        }

        public override bool Intersects(IRectangle rectangle)
        {
            return Intersects(IntersectionUtils.RectangleWithRectangle, rectangle);
        }

        public override bool Intersects(ICircle circle)
        {
            return Intersects(IntersectionUtils.RectangleWithCircle, circle);
        }

        private bool Intersects<TOther>(IntersectionDelegate<IRectangle, TOther> intersectionDelegate, TOther other)
            where TOther : IShape
        {
            Point topleft = Grid.ToGridPoint(other.Center - other.BoundingBox.Size / 2);
            Point bottomRight = Grid.ToGridPoint(other.Center + other.BoundingBox.Size / 2);

            for (int i = topleft.Y; i <= bottomRight.Y; i++)
                for (int j = topleft.X; j <= bottomRight.X; j++)
                {
                    if (!IsCollidableCase(i, j))
                        continue;

                    IRectangle rectangle = new OriginRectangle(Grid.ToWorldPoint(i, j), Grid.Delta);

                    if (intersectionDelegate(rectangle, other))
                        return true;
                }

            return false;
        }

        public bool IsCollidableCase(int i, int j)
        {
            return IsCollidableCaseDelegate(Grid[i, j]);
        }

        public override bool ContainsPoint(Vector2 point)
        {
            return IsCollidableCaseDelegate(Grid[point]);
        }
    }
}