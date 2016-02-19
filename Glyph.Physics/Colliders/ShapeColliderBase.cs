using Glyph.Composition;
using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Physics.Colliders
{
    public abstract class ShapeColliderBase<TShape> : ColliderBase, ICollider<TShape>
        where TShape : IShape
    {
        public abstract TShape Shape { get; }

        protected ShapeColliderBase(Context context)
            : base(context)
        {
        }

        public override bool ContainsPoint(Vector2 point)
        {
            return Shape.ContainsPoint(point);
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