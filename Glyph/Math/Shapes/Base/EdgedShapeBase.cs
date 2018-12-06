using System.Collections.Generic;
using System.Linq;
using Diese.Collections;
using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes.Base
{
    public abstract class EdgedShapeBase : IEdgedShape
    {
        public bool IsVoid => Vertices.Distinct().AtLeast(2);
        public Vector2 Center => MathUtils.GetCenter(Vertices);
        public TopLeftRectangle BoundingBox => MathUtils.GetBoundingBox(Vertices);

        public abstract IEnumerable<Vector2> Vertices { get; }
        public abstract IEnumerable<Segment> Edges { get; }

        public abstract bool ContainsPoint(Vector2 point);
        public bool Intersects(Segment segment) => IntersectionUtils.Intersects(this, segment);
        public bool Intersects<T>(T edgedShape) where T : IEdgedShape => IntersectionUtils.Intersects(this, edgedShape);
        public bool Intersects(Circle circle) => IntersectionUtils.Intersects(this, circle);
    }
}