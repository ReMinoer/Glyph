using System;
using System.Collections.Generic;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Space.Partitioning
{
    public class QuadtreeSpace<T> : SpacePartitionBase<T>
    {
        private readonly IRectangle _bounds;

        public QuadtreeSpace(IRectangle bounds, int capacity, Func<T, Vector2> getPoint)
            : base(null, getPoint, capacity)
        {
            _bounds = bounds;
        }

        public QuadtreeSpace(IRectangle bounds, int capacity, Func<T, IRectangle> getBox)
            : base(null, getBox, capacity)
        {
            _bounds = bounds;
        }

        public QuadtreeSpace(IRectangle bounds, int capacity, Func<T, Vector2> getPoint, Func<T, IRectangle> getBox)
            : base(null, getPoint, getBox, capacity)
        {
            _bounds = bounds;
        }

        private QuadtreeSpace(ISpace<T> parent, IRectangle bounds, int capacity, Func<T, Vector2> getPoint, Func<T, IRectangle> getBox)
            : base(parent, getPoint, getBox, capacity)
        {
            _bounds = bounds;
        }

        public override IRectangle BoundingBox
        {
            get { return _bounds; }
        }

        public override bool ContainsPoint(Vector2 position)
        {
            return _bounds.ContainsPoint(position);
        }

        protected override bool Intersects(IShape range)
        {
            return range.Intersects(_bounds);
        }

        protected override IEnumerable<ISpace<T>> Subdivide()
        {
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                {
                    IRectangle rectangle = new OriginRectangle
                    {
                        Origin = _bounds.Origin + new Vector2(j, i) * _bounds.Size / 2,
                        Size = _bounds.Size / 2
                    };

                    yield return new QuadtreeSpace<T>(this, rectangle, Capacity, GetPoint, GetBox);
                }
        }
    }
}