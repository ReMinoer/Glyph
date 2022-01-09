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

        static public ITransformer Inverse(this ITransformer transformer)
        {
            return new InverseTransformer(transformer);
        }

        private class InverseTransformer : ITransformer
        {
            private readonly ITransformer _transformer;
            public event EventHandler TransformationChanged;

            public InverseTransformer(ITransformer transformer)
            {
                _transformer = transformer;
                _transformer.TransformationChanged += OnTransformationChanged;
            }

            private void OnTransformationChanged(object sender, EventArgs e) => TransformationChanged?.Invoke(this, EventArgs.Empty);

            public Vector2 Transform(Vector2 position) => _transformer.InverseTransform(position);
            public Vector2 InverseTransform(Vector2 position) => _transformer.Transform(position);
            public ITransformation Transform(ITransformation transformation) => _transformer.InverseTransform(transformation);
            public ITransformation InverseTransform(ITransformation transformation) => _transformer.Transform(transformation);
        }

        static public IEdgedShape Transform(this ITransformer transformer, IEdgedShape shape)
        {
            return new TransformedEdgedShape(shape, transformer);
        }

        static public ITriangulableShape Transform(this ITransformer transformer, ITriangulableShape shape)
        {
            Vector2[] vertices = shape.Vertices.Select(transformer.Transform).ToArray();
            Segment[] edges = shape.Edges.Select(transformer.Transform).ToArray();
            return new IndexedShape(vertices, edges, shape.TriangulationIndices?.ToArray(), shape.StripTriangulation);
        }

        static public Segment Transform(this ITransformer transformer, Segment segment)
        {
            return new Segment(transformer.Transform(segment.P0), transformer.Transform(segment.P1));
        }

        static public Triangle Transform(this ITransformer transformer, Triangle triangle)
        {
            return new Triangle(transformer.Transform(triangle.P0), transformer.Transform(triangle.P1), transformer.Transform(triangle.P2));
        }

        static public Quad Transform(this ITransformer transformer, IRectangle rectangle)
        {
            return new Quad(transformer.Transform(rectangle.Position), transformer.Transform(rectangle.P1), transformer.Transform(rectangle.P2));
        }

        static public Quad Transform(this ITransformer transformer, TopLeftRectangle topLeftRectangle)
        {
            return new Quad(transformer.Transform(topLeftRectangle.Position), transformer.Transform(topLeftRectangle.P1), transformer.Transform(topLeftRectangle.P2));
        }

        static public Quad Transform(this ITransformer transformer, CenteredRectangle centeredRectangle)
        {
            return new Quad(transformer.Transform(centeredRectangle.Position), transformer.Transform(centeredRectangle.P1), transformer.Transform(centeredRectangle.P2));
        }

        static public Quad Transform(this ITransformer transformer, Quad quad)
        {
            return new Quad(transformer.Transform(quad.P0), transformer.Transform(quad.P1), transformer.Transform(quad.P2));
        }

        static public Circle Transform(this ITransformer transformer, Circle circle)
        {
            return new Circle(transformer.Transform(circle.Center), transformer.Transform(circle.Center + Vector2.UnitX * circle.Radius).Length());
        }

        static public IEdgedShape InverseTransform(this ITransformer transformer, IEdgedShape shape)
        {
            return new TransformedEdgedShape(shape, transformer.Inverse());
        }

        static public ITriangulableShape InverseTransform(this ITransformer transformer, ITriangulableShape shape)
        {
            Vector2[] vertices = shape.Vertices.Select(transformer.InverseTransform).ToArray();
            Segment[] edges = shape.Edges.Select(transformer.InverseTransform).ToArray();
            return new IndexedShape(vertices, edges, shape.TriangulationIndices?.ToArray(), shape.StripTriangulation);
        }

        static public Segment InverseTransform(this ITransformer transformer, Segment segment)
        {
            return new Segment(transformer.InverseTransform(segment.P0), transformer.InverseTransform(segment.P1));
        }

        static public Triangle InverseTransform(this ITransformer transformer, Triangle triangle)
        {
            return new Triangle(transformer.InverseTransform(triangle.P0), transformer.InverseTransform(triangle.P1), transformer.InverseTransform(triangle.P2));
        }

        static public Quad InverseTransform(this ITransformer transformer, IRectangle rectangle)
        {
            return new Quad(transformer.InverseTransform(rectangle.Position), transformer.InverseTransform(rectangle.P1), transformer.InverseTransform(rectangle.P2));
        }

        static public Quad InverseTransform(this ITransformer transformer, TopLeftRectangle topLeftRectangle)
        {
            return new Quad(transformer.InverseTransform(topLeftRectangle.Position), transformer.InverseTransform(topLeftRectangle.P1), transformer.InverseTransform(topLeftRectangle.P2));
        }

        static public Quad InverseTransform(this ITransformer transformer, CenteredRectangle centeredRectangle)
        {
            return new Quad(transformer.InverseTransform(centeredRectangle.Position), transformer.InverseTransform(centeredRectangle.P1), transformer.InverseTransform(centeredRectangle.P2));
        }

        static public Quad InverseTransform(this ITransformer transformer, Quad quad)
        {
            return new Quad(transformer.InverseTransform(quad.P0), transformer.InverseTransform(quad.P1), transformer.InverseTransform(quad.P2));
        }

        static public Circle InverseTransform(this ITransformer transformer, Circle circle)
        {
            return new Circle(transformer.InverseTransform(circle.Center), transformer.InverseTransform(circle.Center + Vector2.UnitX * circle.Radius).Length());
        }

        private class TransformedEdgedShape : IEdgedShape
        {
            private readonly IEdgedShape _shape;
            protected readonly ITransformer _transformation;

            public IEnumerable<Vector2> Vertices => _shape.Vertices.Select(_transformation.Transform);
            public IEnumerable<Segment> Edges => _shape.Edges.Select(_transformation.Transform);
            public int VertexCount => _shape.VertexCount;
            public int EdgeCount => _shape.EdgeCount;

            public bool IsVoid => Vertices.Distinct().AtLeast(2);
            public Vector2 Center => MathUtils.GetCenter(Vertices);
            public TopLeftRectangle BoundingBox => MathUtils.GetBoundingBox(Vertices);

            public TransformedEdgedShape(IEdgedShape shape, ITransformer transformation)
            {
                _shape = shape;
                _transformation = transformation;
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
                TriangulationIndices = triangulationIndices?.AsReadOnly();
                VertexCount = vertices.Length;
                EdgeCount = edges.Length;
                TriangleCount = triangulationIndices == null ? 0 : (stripTriangulation ? triangulationIndices.Length - 2 : triangulationIndices.Length / 3);
                StripTriangulation = stripTriangulation;
            }
        }
    }
}