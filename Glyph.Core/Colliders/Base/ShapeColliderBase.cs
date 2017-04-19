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

        public override bool Intersects(TopLeftRectangle rectangle)
        {
            return Shape.Intersects(rectangle);
        }

        public override bool Intersects(Circle circle)
        {
            return Shape.Intersects(circle);
        }

        ISceneNode IBoxedComponent.SceneNode
        {
            get { return new ReadOnlySceneNode(SceneNode); }
        }

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