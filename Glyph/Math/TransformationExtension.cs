using System;
using System.Collections.Generic;
using System.Linq;
using Diese.Collections;
using Glyph.Math.Shapes;
using Glyph.Math.Shapes.Base;
using Microsoft.Xna.Framework;

namespace Glyph.Math
{
    static public class TransformationExtension
    {
        static public Transformation Inverse(this ITransformation transformation)
        {
            return new Transformation(-transformation.Translation, -transformation.Rotation, 1 / transformation.Scale);
        }

        static public IEdgedShape Transform(this ITransformation transformation, IEdgedShape shape)
        {
            return new TransformedEdgedShape(shape, transformation);
        }

        static public ITriangulableShape Transform(this ITransformation transformation, ITriangulableShape shape)
        {
            Vector2[] vertices = shape.Vertices.Select(transformation.Transform).ToArray();
            Segment[] edges = shape.Edges.Select(transformation.Transform).ToArray();
            return new IndexedShape(vertices, edges, shape.TriangulationIndices.ToArray(), shape.StripTriangulation);
        }

        static public Segment Transform(this ITransformation transformation, Segment segment)
        {
            return new Segment(transformation.Transform(segment.P0), transformation.Transform(segment.P1));
        }

        static public Triangle Transform(this ITransformation transformation, Triangle triangle)
        {
            return new Triangle(transformation.Transform(triangle.P0), transformation.Transform(triangle.P1), transformation.Transform(triangle.P2));
        }

        static public Quad Transform(this ITransformation transformation, IRectangle rectangle)
        {
            return new Quad(transformation.Transform(rectangle.Position), transformation.Transform(rectangle.P1), transformation.Transform(rectangle.P2));
        }

        static public Quad Transform(this ITransformation transformation, TopLeftRectangle topLeftRectangle)
        {
            return new Quad(transformation.Transform(topLeftRectangle.Position), transformation.Transform(topLeftRectangle.P1), transformation.Transform(topLeftRectangle.P2));
        }

        static public Quad Transform(this ITransformation transformation, CenteredRectangle centeredRectangle)
        {
            return new Quad(transformation.Transform(centeredRectangle.Position), transformation.Transform(centeredRectangle.P1), transformation.Transform(centeredRectangle.P2));
        }

        static public Quad Transform(this ITransformation transformation, Quad quad)
        {
            return new Quad(transformation.Transform(quad.P0), transformation.Transform(quad.P1), transformation.Transform(quad.P2));
        }

        static public Circle Transform(this ITransformation transformation, Circle circle)
        {
            return new Circle(transformation.Transform(circle.Center), circle.Radius * transformation.Scale);
        }

        static public IEdgedShape InverseTransform(this ITransformation transformation, IEdgedShape shape)
        {
            return new TransformedEdgedShape(shape, transformation.Inverse());
        }

        static public ITriangulableShape InverseTransform(this ITransformation transformation, ITriangulableShape shape)
        {
            Vector2[] vertices = shape.Vertices.Select(transformation.InverseTransform).ToArray();
            Segment[] edges = shape.Edges.Select(transformation.InverseTransform).ToArray();
            return new IndexedShape(vertices, edges, shape.TriangulationIndices.ToArray(), shape.StripTriangulation);
        }

        static public Segment InverseTransform(this ITransformation transformation, Segment segment)
        {
            return new Segment(transformation.InverseTransform(segment.P0), transformation.InverseTransform(segment.P1));
        }

        static public Triangle InverseTransform(this ITransformation transformation, Triangle triangle)
        {
            return new Triangle(transformation.InverseTransform(triangle.P0), transformation.InverseTransform(triangle.P1), transformation.InverseTransform(triangle.P2));
        }

        static public Quad InverseTransform(this ITransformation transformation, IRectangle rectangle)
        {
            return new Quad(transformation.InverseTransform(rectangle.Position), transformation.InverseTransform(rectangle.P1), transformation.InverseTransform(rectangle.P2));
        }

        static public Quad InverseTransform(this ITransformation transformation, TopLeftRectangle topLeftRectangle)
        {
            return new Quad(transformation.InverseTransform(topLeftRectangle.Position), transformation.InverseTransform(topLeftRectangle.P1), transformation.InverseTransform(topLeftRectangle.P2));
        }

        static public Quad InverseTransform(this ITransformation transformation, CenteredRectangle centeredRectangle)
        {
            return new Quad(transformation.InverseTransform(centeredRectangle.Position), transformation.InverseTransform(centeredRectangle.P1), transformation.InverseTransform(centeredRectangle.P2));
        }

        static public Quad InverseTransform(this ITransformation transformation, Quad quad)
        {
            return new Quad(transformation.InverseTransform(quad.P0), transformation.InverseTransform(quad.P1), transformation.InverseTransform(quad.P2));
        }

        static public Circle InverseTransform(this ITransformation transformation, Circle circle)
        {
            return new Circle(transformation.InverseTransform(circle.Center), circle.Radius / transformation.Scale);
        }

        private class TransformedEdgedShape : IEdgedShape
        {
            private readonly IEdgedShape _shape;
            protected readonly ITransformation _transformation;

            public IEnumerable<Vector2> Vertices => _shape.Vertices.Select(_transformation.Transform);
            public IEnumerable<Segment> Edges => _shape.Edges.Select(_transformation.Transform);
            public int VertexCount => _shape.VertexCount;
            public int EdgeCount => _shape.EdgeCount;

            public bool IsVoid => Vertices.Distinct().AtLeast(2);
            public Vector2 Center => MathUtils.GetCenter(Vertices);
            public TopLeftRectangle BoundingBox => MathUtils.GetBoundingBox(Vertices);

            public TransformedEdgedShape(IEdgedShape shape, ITransformation transformation)
            {
                _shape = shape;
                _transformation = new Transformation(transformation);
            }

            public bool ContainsPoint(Vector2 point) => _shape.ContainsPoint(_transformation.InverseTransform(point));
            public bool Intersects(Segment segment) => _shape.Intersects(_transformation.InverseTransform(segment));
            public bool Intersects<T>(T edgedShape) where T : IEdgedShape => _shape.Intersects(_transformation.InverseTransform(edgedShape));
            public bool Intersects(Circle circle) => _shape.Intersects(_transformation.InverseTransform(circle));
        }

        private class IndexedShape : IndexedShapeBase
        {
            private readonly Vector2[] _vertices;
            public override IEnumerable<Vector2> Vertices { get; }
            public override IEnumerable<Segment> Edges { get; }
            public override IEnumerable<ushort> TriangulationIndices { get; }
            public override int VertexCount { get; }
            public override int EdgeCount { get; }
            public override int TriangleCount { get; }
            public override bool StripTriangulation { get; }

            public override Vector2 GetIndexedVertex(ushort index) => _vertices[index];

            public IndexedShape(Vector2[] vertices, Segment[] edges, ushort[] triangulationIndices, bool stripTriangulation)
            {
                _vertices = vertices;

                Vertices = vertices.AsReadOnly();
                Edges = edges.AsReadOnly();
                TriangulationIndices = triangulationIndices.AsReadOnly();
                VertexCount = vertices.Length;
                EdgeCount = edges.Length;
                TriangleCount = stripTriangulation ? triangulationIndices.Length - 2 : triangulationIndices.Length / 3;
                StripTriangulation = stripTriangulation;
            }
        }
    }
}