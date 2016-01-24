using System.Collections.Generic;
using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Space.Partitioning
{
    public interface ISpacePartitioner<T> : IArea
    {
        SpacePartitionerParameters Parameters { get; }
        bool Insert(T item);
        bool Remove(T item);
        IPartition<T> GetPartition(Vector2 position);
        IEnumerable<T> GetAllPointsInRange(IShape range);
    }
}