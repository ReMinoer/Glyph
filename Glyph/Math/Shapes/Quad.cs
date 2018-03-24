using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public struct Quad : ITriangledShape
    {
        public Vector2 P0 { get; set; }
        public Vector2 P1 { get; set; }
        public Vector2 P2 { get; set; }
        public Vector2 P3 => P1 + P2 - P0;

        public bool IsVoid => Size.X.EqualsZero() || Size.Y.EqualsZero();
        public Vector2 Size => new Vector2(System.Math.Abs((P1 - P0).X), System.Math.Abs((P2 - P0).Y));
        public Vector2 Center => (P1 - P0 + P2 - P0) / 2;
        public TopLeftRectangle BoundingBox => MathUtils.GetBoundingBox(P0, P1, P2, P3);

        public IEnumerable<Vector2> Vertices
        {
            get
            {
                yield return P0;
                yield return P1;
                yield return P2;
                yield return P3;
            }
        }

        public IEnumerable<Segment> Edges
        {
            get
            {
                yield return new Segment(P0, P1);
                yield return new Segment(P1, P2);
                yield return new Segment(P2, P3);
                yield return new Segment(P3, P0);
            }
        }

        public IEnumerable<Triangle> Triangles
        {
            get
            {
                yield return new Triangle(P0, P1, P2);
                yield return new Triangle(P3, P2, P1);
            }
        }

        public bool ContainsPoint(Vector2 point) => Triangles.Any(t => t.ContainsPoint(point));

        public bool Intersects(Segment segment) => IntersectionUtils.Intersects(this, segment);
        public bool Intersects<T>(T edgedShape) where T : IEdgedShape => IntersectionUtils.Intersects(this, edgedShape);
        public bool Intersects(Circle circle) => IntersectionUtils.Intersects(this, circle);
    }
}