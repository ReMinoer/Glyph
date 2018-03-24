using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public struct Triangle : ITriangledShape
    {
        public Vector2 P0 { get; set; }
        public Vector2 P1 { get; set; }
        public Vector2 P2 { get; set; }

        public Vector2 Center => (P0 + P1 + P2) / 3;
        public TopLeftRectangle BoundingBox => MathUtils.GetBoundingBox(P0, P1, P2);

        public IEnumerable<Vector2> Vertices
        {
            get
            {
                yield return P0;
                yield return P1;
                yield return P2;
            }
        }

        public IEnumerable<Segment> Edges
        {
            get
            {
                yield return new Segment(P0, P1);
                yield return new Segment(P1, P2);
                yield return new Segment(P2, P0);
            }
        }

        IEnumerable<Triangle> ITriangledShape.Triangles
        {
            get
            {
                yield return this;
            }
        }

        public Triangle(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            P0 = p0;
            P1 = p1;
            P2 = p2;
        }

        public bool IsVoid
        {
            get
            {
                float dot = Vector2.Dot(P1 - P0, P2 - P0);
                return dot.EqualsZero() || dot.EpsilonEquals(1);
            }
        }

        public bool ContainsPoint(Vector2 point)
        {
            Vector2 v0 = P2 - P0;
            Vector2 v1 = P1 - P0;
            Vector2 v2 = point - P0;

            float dot00 = Vector2.Dot(v0, v0);
            float dot01 = Vector2.Dot(v0, v1);
            float dot02 = Vector2.Dot(v0, v2);
            float dot11 = Vector2.Dot(v1, v1);
            float dot12 = Vector2.Dot(v1, v2);

            // Compute barycentric coordinates
            float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            // Check if point is in triangle
            return u >= 0 && v >= 0 && u + v < 1;
        }

        public bool Intersects(Segment segment) => IntersectionUtils.Intersects(this, segment);
        public bool Intersects<T>(T edgedShape) where T : IEdgedShape => IntersectionUtils.Intersects(this, edgedShape);
        public bool Intersects(Circle circle) => IntersectionUtils.Intersects(this, circle);
    }
}