using System;
using Glyph.Core.Colliders.Base;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Space;
using Microsoft.Xna.Framework;

namespace Glyph.Core.Colliders
{
    public class GridCollider<TData> : ColliderBase, IGridCollider
    {
        public IGrid<TData> Grid { get; set; }

        public Func<ICollider, TData, bool> IsCollidableCaseDelegate { get; set; }

        IGrid IGridCollider.Grid
        {
            get { return Grid; }
        }

        public override TopLeftRectangle BoundingBox
        {
            get { return Grid.BoundingBox; }
        }

        public GridCollider(ColliderManager colliderManager)
            : base(colliderManager)
        {
        }

        protected override bool IsColliding(RectangleCollider collider, out Collision collision)
        {
            return CollisionUtils.IsGridCollidingShape<TopLeftRectangle>(IntersectionUtils.RectangleWithRectangle, this, collider, out collision);
        }

        protected override bool IsColliding(CircleCollider collider, out Collision collision)
        {
            return CollisionUtils.IsGridCollidingShape(IntersectionUtils.RectangleWithCircle, this, collider, out collision);
        }

        protected override bool IsColliding(IGridCollider collider, out Collision collision)
        {
            throw new NotImplementedException();
        }

        public override bool Intersects(TopLeftRectangle rectangle)
        {
            return Intersects(IntersectionUtils.RectangleWithRectangle, rectangle);
        }

        public override bool Intersects(Circle circle)
        {
            return Intersects(IntersectionUtils.RectangleWithCircle, circle);
        }

        private bool Intersects<TOther>(IntersectionDelegate<TopLeftRectangle, TOther> intersectionDelegate, TOther other)
            where TOther : IShape
        {
            Point topleft = Grid.ToGridPoint(other.Center - other.BoundingBox.Size / 2);
            Point bottomRight = Grid.ToGridPoint(other.Center + other.BoundingBox.Size / 2);

            for (int i = topleft.Y; i <= bottomRight.Y; i++)
                for (int j = topleft.X; j <= bottomRight.X; j++)
                {
                    if (!IsCollidableCase(null, i, j))
                        continue;

                    TopLeftRectangle rectangle = new TopLeftRectangle(Grid.ToWorldPoint(i, j), Grid.Delta);

                    if (intersectionDelegate(rectangle, other))
                        return true;
                }

            return false;
        }

        public bool IsCollidableCase(ICollider collider, int i, int j)
        {
            return IsCollidableCaseDelegate(collider, Grid[i, j]);
        }

        public override bool ContainsPoint(Vector2 point)
        {
            return IsCollidableCaseDelegate(null, Grid[point]);
        }
    }
}