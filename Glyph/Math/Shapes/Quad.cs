using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public struct Quad : IRectangle
    {
        public Vector2 P0 { get; set; }
        public Vector2 P1 { get; set; }
        public Vector2 P2 { get; set; }
        public Vector2 P3 => P1 + P2 - P0;

        public bool IsVoid => Size.X.EqualsZero() || Size.Y.EqualsZero();
        public Vector2 Size => new Vector2(Width, Height);
        public Vector2 Center => (P1 + P2) / 2;
        public TopLeftRectangle BoundingBox => MathUtils.GetBoundingBox(Vertices);

        public Vector2 TopLeft => new Vector2(Left, Top);
        public Vector2 TopRight => new Vector2(Right, Top);
        public Vector2 BottomLeft => new Vector2(Left, Bottom);
        public Vector2 BottomRight => new Vector2(Right, Bottom);

        Vector2 IRectangle.Position => TopLeft;
        public float Left => Vertices.Select(v => v.X).Min();
        public float Right => Vertices.Select(v => v.X).Max();
        public float Top => Vertices.Select(v => v.Y).Min();
        public float Bottom => Vertices.Select(v => v.Y).Max();
        public float Width => System.Math.Abs((P1 - P0).X);
        public float Height => System.Math.Abs((P2 - P0).Y);

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

        public Vector2 GetIndexedVertex(ushort index)
        {
            switch (index)
            {
                case 0: return P0;
                case 1: return P1;
                case 2: return P2;
                case 3: return P3;
                default: throw new IndexOutOfRangeException();
            }
        }

        public IEnumerable<Segment> Edges
        {
            get
            {
                yield return new Segment(P0, P1);
                yield return new Segment(P1, P3);
                yield return new Segment(P3, P2);
                yield return new Segment(P2, P0);
            }
        }
        
        bool ITriangulableShape.StripTriangulation => true;
        IEnumerable<ushort> ITriangulableShape.TriangulationIndices => null;
        
        public int VertexCount => 4;
        public int EdgeCount => 4;
        public int TriangleCount => 2;

        public Quad(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            P0 = p0;
            P1 = p1;
            P2 = p2;
        }

        public bool ContainsPoint(Vector2 point) => this.Triangles().Any(t => t.ContainsPoint(point));

        public bool Intersects(Segment segment) => IntersectionUtils.Intersects(this, segment);
        public bool Intersects<T>(T edgedShape) where T : IEdgedShape => IntersectionUtils.Intersects(this, edgedShape);
        public bool Intersects(Circle circle) => IntersectionUtils.Intersects(this, circle);
    }
}