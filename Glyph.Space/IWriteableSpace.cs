using System.Collections.Generic;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public interface IWriteableSpace<T> : ISpace<T>, ICollection<T>
    {
        new IEnumerable<IWriteableSpace<T>> Partitions { get; }
        new IWriteableSpace<T> GetPartition(Vector2 position);
        new IWriteableSpace<T> GetBestPartition(Vector2 position);
        new IEnumerable<IWriteableSpace<T>> GetAllPartitionsInRange(TopLeftRectangle range);
        new bool Add(T item);
    }
}