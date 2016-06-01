using System.Collections.Generic;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public interface ISpace : IArea
    {
        IEnumerable<Vector2> Points { get; }
        IEnumerable<IRectangle> Boxes { get; }
        IEnumerable<ISpace> Partitions { get; }
        ISpace GetPartition(Vector2 position);
        ISpace GetBestPartition(Vector2 position);
        IEnumerable<ISpace> GetAllPartitionsInRange(IRectangle range);
        IEnumerable<Vector2> GetAllPointsInRange(IShape range);
        IEnumerable<IRectangle> GetAllBoxesInRange(IShape range);
    }

    public interface ISpace<T> : ISpace, ICollection<T>
    {
        new IEnumerable<ISpace<T>> Partitions { get; }
        new bool Add(T item);
        new ISpace<T> GetPartition(Vector2 position);
        new ISpace<T> GetBestPartition(Vector2 position);
        new IEnumerable<ISpace<T>> GetAllPartitionsInRange(IRectangle range);
        IEnumerable<T> GetAllItemsInRange(IShape range);
    }
}