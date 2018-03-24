using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Core.Colliders.Base
{
    public abstract class ShapeColliderBase<TShape> : ColliderBase, ICollider<TShape>
        where TShape : IShape
    {
        public abstract TShape Shape { get; }
        public override bool IsVoid => Shape.IsVoid;

        public override TopLeftRectangle BoundingBox
        {
            get { return Shape.BoundingBox; }
        }

        protected ShapeColliderBase(ColliderManager colliderManager)
            : base(colliderManager)
        {
        }

        public override bool ContainsPoint(Vector2 point)
        {
            return Shape.ContainsPoint(point);
        }

        public override bool Intersects(Segment segment) => Shape.Intersects(segment);
        public override bool Intersects<T>(T edgedShape) => Shape.Intersects(edgedShape);
        public override bool Intersects(Circle circle) => Shape.Intersects(circle);

        IShape IShapedComponent.Shape
        {
            get { return Shape; }
        }

        IArea IBoxedComponent.Area
        {
            get { return Shape; }
        }
    }
}