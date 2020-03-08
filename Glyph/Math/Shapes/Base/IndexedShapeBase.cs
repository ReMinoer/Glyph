using System.Collections.Generic;
using System.Linq;
using Diese.Collections;
using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes.Base
{
    public abstract class IndexedShapeBase : ITriangulableShape
    {
        public bool IsVoid => Vertices.Distinct().AtLeast(2);
        public Vector2 Center => MathUtils.GetCenter(Vertices);
        public TopLeftRectangle BoundingBox => MathUtils.GetBoundingBox(Vertices);

        public abstract IEnumerable<Vector2> Vertices { get; }
        public abstract IEnumerable<Segment> Edges { get; }
        public abstract IEnumerable<ushort> TriangulationIndices { get; }
        public abstract int VertexCount { get; }
        public abstract int EdgeCount { get; }
        public abstract int TriangleCount { get; }
        public abstract bool StripTriangulation { get; }
        public abstract Vector2 GetIndexedVertex(ushort index);

        public bool ContainsPoint(Vector2 point) => this.Triangles().Any(t => t.ContainsPoint(point));
        public bool Intersects(Segment segment) => IntersectionUtils.Intersects(this, segment);
        public bool Intersects<T>(T edgedShape) where T : IEdgedShape => IntersectionUtils.Intersects(this, edgedShape);
        public bool Intersects(Circle circle) => IntersectionUtils.Intersects(this, circle);
    }
}