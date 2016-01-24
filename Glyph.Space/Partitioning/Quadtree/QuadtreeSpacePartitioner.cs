using System;
using Glyph.Space.Partitioning.Base;
using Microsoft.Xna.Framework;

namespace Glyph.Space.Partitioning.Quadtree
{
    public class QuadtreeSpacePartitioner<T> : SpacePartitionerBase<T>
    {
        private readonly QuadtreePartition<T> _root;

        protected override PartitionBase<T> Root
        {
            get { return _root; }
        }

        public QuadtreeSpacePartitioner(Func<T, Vector2> getPosition, SpacePartitionerParameters parameters)
            : base(getPosition, parameters)
        {
            _root = new QuadtreePartition<T>(getPosition, null, Parameters.NodeCapacity, Parameters.Bounds);
        }
    }
}