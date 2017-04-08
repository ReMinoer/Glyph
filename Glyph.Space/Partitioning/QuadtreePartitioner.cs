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

        public QuadtreePartitioner(TopLeftRectangle bounds, int capacity)
        {
            BoundingBox = bounds;
            Capacity = capacity;
        }

        public bool ContainsPoint(Vector2 position)
        {
            return BoundingBox.ContainsPoint(position);
        }

        public bool Intersects(IShape range)
        {
            return range.Intersects(BoundingBox);
        }

        public bool Intersects(TopLeftRectangle rectangle)
        {
            return BoundingBox.Intersects(rectangle);
        }

        public bool Intersects(Circle circle)
        {
            return BoundingBox.Intersects(circle);
        }

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