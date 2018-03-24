using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public struct Segment : ITriangledShape
    {
        public Vector2 P0 { get; set; }
        public Vector2 P1 { get; set; }
        public float Length => (P1 - P0).Length();
        public Vector2 Vector => P1 - P0;
        public Vector2 Center => (P0 + P1) / 2;

        bool IArea.IsVoid => true;
        TopLeftRectangle IArea.BoundingBox => MathUtils.GetBoundingBox(P0, P1);

        public IEnumerable<Vector2> Vertices
        {
            get
            {
                yield return P0;
                yield return P1;
            }
        }

        IEnumerable<Segment> IEdgedShape.Edges
        {
            get
            {
                yield return this;
            }
        }

        IEnumerable<Triangle> ITriangledShape.Triangles
        {
            get
            {
                yield break;
            }
        }

        public Segment(Vector2 p0, Vector2 p1)
        {
            P0 = p0;
            P1 = p1;
        }

        // https://stackoverflow.com/a/328122/3333753
        public bool ContainsPoint(Vector2 point)
        {
            Vector2 a = point - P0;
            Vector2 b = P1 - P0;

            // Check if the points are aligned
            if (!a.Cross(b).EqualsZero())
                return false;

            // Check if the point is on the line segment
            float dotProduct = Vector2.Dot(a, b);
            if (Vector2.Dot(a, b) < 0)
                return false;

            return b.LengthSquared() > dotProduct;
        }

        public bool Intersects(Segment segment) => IntersectionUtils.Intersects(this, segment);
        public bool Intersects<T>(T edgedShape) where T : IEdgedShape => IntersectionUtils.Intersects(this, edgedShape);
        public bool Intersects(Circle circle) => IntersectionUtils.Intersects(this, circle);
    }
}