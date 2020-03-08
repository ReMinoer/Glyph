using System.Collections.Generic;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Space.Partitioning
{
    public class QuadtreePartitioner : IPartitioner
    {
        public TopLeftRectangle BoundingBox { get; }
        public int Capacity { get; }

        public Vector2 Center => BoundingBox.Center;
        public bool IsVoid => BoundingBox.IsVoid;
        IEnumerable<Vector2> IEdgedShape.Vertices => BoundingBox.Vertices;
        IEnumerable<Segment> IEdgedShape.Edges => BoundingBox.Edges;
        int IEdgedShape.VertexCount => BoundingBox.VertexCount;
        int IEdgedShape.EdgeCount => BoundingBox.EdgeCount;

        public QuadtreePartitioner(TopLeftRectangle bounds, int capacity)
        {
            BoundingBox = bounds;
            Capacity = capacity;
        }

        public bool ContainsPoint(Vector2 position) => BoundingBox.ContainsPoint(position);
        public bool Intersects(Segment segment) => BoundingBox.Intersects(segment);
        public bool Intersects<T>(T edgedShape) where T : IEdgedShape => BoundingBox.Intersects(edgedShape);
        public bool Intersects(Circle circle) => BoundingBox.Intersects(circle);

        public IEnumerable<IPartitioner> Subdivide()
        {
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                {
                    var rectangle = new TopLeftRectangle
                    {
                        Position = BoundingBox.Position + new Vector2(j, i) * BoundingBox.Size / 2,
                        Size = BoundingBox.Size / 2
                    };

                    yield return new QuadtreePartitioner(rectangle, Capacity);
                }
        }
    }
}