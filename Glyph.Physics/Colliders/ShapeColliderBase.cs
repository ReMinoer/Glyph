using Glyph.Composition;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Physics.Colliders
{
    public abstract class ShapeColliderBase<TShape> : ColliderBase, ICollider<TShape>
        where TShape : IShape
    {
        public abstract TShape Shape { get; }

        public override IRectangle BoundingBox
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

        public override bool Intersects(IRectangle rectangle)
        {
            return Shape.Intersects(rectangle);
        }

        public override bool Intersects(ICircle circle)
        {
            return Shape.Intersects(circle);
        }

        ISceneNode IShapedObject.SceneNode
        {
            get { return new ReadOnlySceneNode(SceneNode); }
        }

        IShape IShapedObject.Shape
        {
            get { return Shape; }
        }
    }
}