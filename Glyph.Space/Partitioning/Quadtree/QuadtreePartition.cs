using System;
using System.Collections.Generic;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Space.Partitioning.Base;
using Microsoft.Xna.Framework;

namespace Glyph.Space.Partitioning.Quadtree
{
    public class QuadtreePartition<T> : PartitionBase<T>
    {
        private readonly IRectangle _bounds;
        private readonly List<QuadtreePartition<T>> _children;

        public QuadtreePartition(Func<T, Vector2> getPosition, IPartition<T> parent, int capacity, IRectangle bounds)
            : base(getPosition, parent, capacity)
        {
            _bounds = bounds;
            _children = new List<QuadtreePartition<T>>();
        }

        public override bool ContainsPoint(Vector2 position)
        {
            return _bounds.ContainsPoint(position);
        }

        protected override bool Intersects(IShape range)
        {
            return range.Intersects(_bounds);
        }

        protected override void Subdivide()
        {
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                {
                    IRectangle rectangle = new OriginRectangle
                    {
                        Origin = _bounds.Origin + new Vector2(j, i) * _bounds.Size / 2,
                        Size = _bounds.Size / 2
                    };

                    _children.Add(new QuadtreePartition<T>(GetPosition, this, Capacity, rectangle));
                }
        }

        public override IEnumerator<IPartition<T>> GetEnumerator()
        {
            return _children.GetEnumerator();
        }
    }
}