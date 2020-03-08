﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Glyph.Math.Shapes
{
    public struct CenteredRectangle : IRectangle
    {
        public Vector2 Center { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        
        public Vector2 Position => new Vector2(Left, Top);
        public Vector2 P1 => new Vector2(Right, Top);
        public Vector2 P2 => new Vector2(Left, Bottom);
        public Vector2 P3 => new Vector2(Right, Bottom);

        public float Left => Center.X - Width / 2;
        public float Right => Center.X + Width / 2;
        public float Top => Center.Y - Height / 2;
        public float Bottom => Center.Y + Height / 2;
        public bool IsVoid => Width.EqualsZero() && Height.EqualsZero();

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

        public CenteredRectangle(Vector2 center, float width, float height)
            : this()
        {
            Center = center;
            Width = width;
            Height = height;
        }

        public CenteredRectangle(Vector2 center, Vector2 size)
            : this()
        {
            Center = center;
            Size = size;
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
            return $"Center: {Center} - Size: {Size}";
        }

        static public implicit operator CenteredRectangle(TopLeftRectangle rectangle)
        {
            return new CenteredRectangle(rectangle.Center, rectangle.Width, rectangle.Height);
        }

        static public implicit operator TopLeftRectangle(CenteredRectangle rectangle)
        {
            return new TopLeftRectangle(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
        }

        static public implicit operator Quad(CenteredRectangle rectangle)
        {
            return new Quad(rectangle.Position, rectangle.P1, rectangle.P2);
        }
    }
}