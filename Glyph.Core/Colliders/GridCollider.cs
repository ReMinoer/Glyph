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
        IGrid IGridCollider.Grid => Grid;

        public Func<ICollider, TData, bool> IsCollidableCaseDelegate { get; set; }

        public override TopLeftRectangle BoundingBox => Grid.BoundingBox;
        public override bool IsVoid => Grid.IsVoid;

        public GridCollider(ColliderManager colliderManager)
            : base(colliderManager)
        {
        }

        protected override bool IsColliding(RectangleCollider collider, out Collision collision) => throw new NotImplementedException();
        protected override bool IsColliding(CircleCollider collider, out Collision collision) => throw new NotImplementedException();
        protected override bool IsColliding(IGridCollider collider, out Collision collision) => throw new NotImplementedException();

        public override bool Intersects(Segment segment) => Intersects(IntersectionUtils.Intersects, segment);
        public override bool Intersects<T>(T edgedShape) => Intersects(IntersectionUtils.Intersects, edgedShape);
        public override bool Intersects(Circle circle) => Intersects(IntersectionUtils.Intersects, circle);

        public bool Intersects<TOther>(IntersectionDelegate<TopLeftRectangle, TOther> intersectionDelegate, TOther other) where TOther : IShape
            => !Grid.Intersection(intersectionDelegate, other, (x, y) => IsCollidableCase(null, x, y)).IsEmpty;

        public bool IsCollidableCase(ICollider collider, int i, int j) => IsCollidableCaseDelegate(collider, Grid[i, j]);
        public override bool ContainsPoint(Vector2 point) => IsCollidableCaseDelegate(null, Grid[point]);
    }
}