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

        static public ITriangledShape Transform(this ITransformation transformation, ITriangledShape shape)
        {
            IEnumerable<Vector2> vertices = shape.Vertices.Select(transformation.Transform);
            IEnumerable<Segment> edges = shape.Edges.Select(transformation.Transform);
            IEnumerable<Triangle> triangles = shape.Triangles.Select(transformation.Transform);
            return new TriangledShape(vertices, edges, triangles);
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

        static public ITriangledShape InverseTransform(this ITransformation transformation, ITriangledShape shape)
        {
            IEnumerable<Vector2> vertices = shape.Vertices.Select(transformation.InverseTransform);
            IEnumerable<Segment> edges = shape.Edges.Select(transformation.InverseTransform);
            IEnumerable<Triangle> triangles = shape.Triangles.Select(transformation.InverseTransform);
            return new TriangledShape(vertices, edges, triangles);
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

        private class TriangledShape : TriangledShapeBase
        {
            public override IEnumerable<Vector2> Vertices { get; }
            public override IEnumerable<Segment> Edges { get; }
            public override IEnumerable<Triangle> Triangles { get; }

            public TriangledShape(IEnumerable<Vector2> vertices, IEnumerable<Segment> edges, IEnumerable<Triangle> triangles)
            {
                Vertices = vertices;
                Edges = edges;
                Triangles = triangles;
            }
        }
    }
}