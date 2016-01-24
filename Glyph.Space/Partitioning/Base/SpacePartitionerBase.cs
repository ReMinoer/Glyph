using System;
using System.Collections.Generic;
using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Space.Partitioning.Base
{
    public abstract class SpacePartitionerBase<T> : ISpacePartitioner<T>
    {
        protected Func<T, Vector2> GetPosition;
        protected abstract PartitionBase<T> Root { get; }
        public SpacePartitionerParameters Parameters { get; private set; }

        protected SpacePartitionerBase(Func<T, Vector2> getPosition, SpacePartitionerParameters parameters)
        {
            GetPosition = getPosition;
            Parameters = parameters;
        }

        public bool ContainsPoint(Vector2 point)
        {
            return Root.ContainsPoint(point);
        }

        public bool Insert(T item)
        {
            return Root.TryInsert(item, 0, Parameters.DepthMax);
        }

        public bool Remove(T item)
        {
            return Root.TryRemove(item);
        }

        public IPartition<T> GetPartition(Vector2 position)
        {
            return Root.GetBestPartition(position);
        }

        public IEnumerable<T> GetAllPointsInRange(IShape range)
        {
            return Root.GetAllPointsInRange(range);
        }
    }
}