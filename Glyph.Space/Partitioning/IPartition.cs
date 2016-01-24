using System.Collections.Generic;
using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Space.Partitioning
{
    public interface IPartition<T> : IEnumerable<IPartition<T>>, IArea
    {
        IPartition<T> Parent { get; }
        int Capacity { get; }
        IEnumerable<T> Items { get; }
        bool TryInsert(T item, int depth, int depthMax);
        bool TryRemove(T item);
        IPartition<T> GetBestPartition(Vector2 position);
        IEnumerable<T> GetAllPointsInRange(IShape range);
    }
}