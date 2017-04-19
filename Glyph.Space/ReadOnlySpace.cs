using System.Collections;
using System.Collections.Generic;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public class ReadOnlySpace : ISpace
    {
        private readonly ISpace _space;

        public ReadOnlySpace(ISpace space)
        {
            _space = space;
        }

        public bool IsVoid => _space.IsVoid;

        public TopLeftRectangle BoundingBox
        {
            get { return _space.BoundingBox; }
        }

        public bool ContainsPoint(Vector2 point)
        {
            return _space.ContainsPoint(point);
        }

        public IEnumerable<Vector2> Points
        {
            get { return _space.Points; }
        }

        public IEnumerable<TopLeftRectangle> Boxes
        {
            get { return _space.Boxes; }
        }

        public IEnumerable<ISpace> Partitions
        {
            get { return _space.Partitions; }
        }

        public ISpace GetPartition(Vector2 position)
        {
            return _space.GetPartition(position);
        }

        public ISpace GetBestPartition(Vector2 position)
        {
            return _space.GetBestPartition(position);
        }

        public IEnumerable<ISpace> GetAllPartitionsInRange(TopLeftRectangle range)
        {
            return _space.GetAllPartitionsInRange(range);
        }

        public IEnumerable<Vector2> GetAllPointsInRange(IShape range)
        {
            return _space.GetAllPointsInRange(range);
        }

        public IEnumerable<TopLeftRectangle> GetAllBoxesInRange(IShape range)
        {
            return _space.GetAllBoxesInRange(range);
        }
    }

    public class ReadOnlySpace<T> : ReadOnlySpace, ISpace<T>
    {
        private readonly ISpace<T> _space;

        public ReadOnlySpace(ISpace<T> space)
            : base(space)
        {
            _space = space;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _space.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_space).GetEnumerator();
        }

        new public IEnumerable<ISpace<T>> Partitions
        {
            get { return _space.Partitions; }
        }

        new public ISpace<T> GetPartition(Vector2 position)
        {
            return _space.GetPartition(position);
        }

        new public ISpace<T> GetBestPartition(Vector2 position)
        {
            return _space.GetBestPartition(position);
        }

        new public IEnumerable<ISpace<T>> GetAllPartitionsInRange(TopLeftRectangle range)
        {
            return _space.GetAllPartitionsInRange(range);
        }

        public IEnumerable<T> GetAllItemsInRange(IShape range)
        {
            return _space.GetAllItemsInRange(range);
        }
    }
}