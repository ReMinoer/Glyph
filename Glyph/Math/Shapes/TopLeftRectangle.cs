using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public struct TopLeftRectangle : IRectangle, IMovableShape
    {
        public float Left { get; set; }
        public float Top { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public Vector2 Position
        {
            get => new Vector2(Left, Top);
            set
            {
                Left = value.X;
                Top = value.Y;
            }
        }

        public Vector2 P1 => new Vector2(Right, Top);
        public Vector2 P2 => new Vector2(Left, Bottom);
        public Vector2 P3 => new Vector2(Right, Bottom);
        public bool IsVoid => Width <= 0 && Height <= 0;

        public Vector2 Center
        {
            get => Position + Size / 2;
            set
            {
                Left = value.X - Width / 2;
                Top = value.Y - Height / 2;
            }
        }

        public float Right
        {
            get => Left + Width;
            set => Width = value - Left;
        }

        public float Bottom
        {
            get => Top + Height;
            set => Height = value - Top;
        }

        public Vector2 Size
        {
            get => new Vector2(Width, Height);
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }

        public IEnumerable<Vector2> Vertices
        {
            get
            {
                yield return Position;
                yield return P1;
                yield return P2;
                yield return P3;
            }
        }

        public Vector2 GetIndexedVertex(ushort index)
        {
            switch (index)
            {
                case 0: return Position;
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
                yield return new Segment(Position, P1);
                yield return new Segment(P1, P3);
                yield return new Segment(P3, P2);
                yield return new Segment(P2, Position);
            }
        }
        
        bool ITriangulableShape.StripTriangulation => true;
        IEnumerable<ushort> ITriangulableShape.TriangulationIndices => null;
        
        public int VertexCount => 4;
        public int EdgeCount => 4;
        public int TriangleCount => 2;

        TopLeftRectangle IArea.BoundingBox => this;

        static public TopLeftRectangle Void => new TopLeftRectangle();

        public TopLeftRectangle(Vector2 origin, Vector2 size)
            : this(origin.X, origin.Y, size.X, size.Y)
        {
        }

        public TopLeftRectangle(float x, float y, float width, float height)
            : this()
        {
            Left = x;
            Top = y;
            Width = width;
            Height = height;
        }

        public bool ContainsPoint(Vector2 point)
        {
            return point.X > Left && point.X < Right && point.Y > Top && point.Y < Bottom;
        }

        public bool Intersects(Segment segment) => IntersectionUtils.Intersects(this, segment);
        public bool Intersects<T>(T edgedShape) where T : IEdgedShape => IntersectionUtils.Intersects(this, edgedShape);
        public bool Intersects(Circle circle) => IntersectionUtils.Intersects(this, circle);

        public override string ToString()
        {
            return $"Origin: {Position} - Size: {Size}";
        }

        static public implicit operator Quad(TopLeftRectangle rectangle)
        {
            return new Quad(rectangle.Position, rectangle.P1, rectangle.P2);
        }
    }
}