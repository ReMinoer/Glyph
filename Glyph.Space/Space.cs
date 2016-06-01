using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public class Space<T> : ISpace<T>
    {
        private readonly List<T> _items;
        private readonly Func<T, Vector2> _getPoint;
        private readonly Func<T, IRectangle> _getBox;

        public IEnumerable<Vector2> Points
        {
            get { return _items.Select(x => _getBox(x).Center); }
        }

        public IEnumerable<IRectangle> Boxes
        {
            get { return _items.Select(x => _getBox(x)); }
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public IRectangle BoundingBox
        {
            get { return MathUtils.GetBoundingBox(_items.Select(x => _getBox(x).Center)); }
        }

        IEnumerable<ISpace<T>> ISpace<T>.Partitions
        {
            get { yield break; }
        }

        IEnumerable<ISpace> ISpace.Partitions
        {
            get { yield break; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        public Space(Func<T, Vector2> getPoint)
            : this(getPoint, x => new CenteredRectangle(getPoint(x), 0, 0))
        {
        }

        public Space(Func<T, IRectangle> getBox)
            : this(x => getBox(x).Center, getBox)
        {
        }

        public Space(Func<T, Vector2> getPoint, Func<T, IRectangle> getBox)
        {
            _getPoint = getPoint;
            _getBox = getBox;
            _items = new List<T>();
        }

        public bool Add(T item)
        {
            _items.Add(item);
            return true;
        }

        public bool Remove(T item)
        {
            return _items.Remove(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public bool ContainsPoint(Vector2 point)
        {
            return BoundingBox.ContainsPoint(point);
        }

        public IEnumerable<Vector2> GetAllPointsInRange(IShape range)
        {
            return GetAllItemsInRange(range).Select(x => _getBox(x).Center);
        }

        public IEnumerable<IRectangle> GetAllBoxesInRange(IShape range)
        {
            foreach (T item in _items)
            {
                IRectangle box = _getBox(item);
                if (range.Intersects(_getBox(item)))
                    yield return box;
            }
        }

        public IEnumerable<T> GetAllItemsInRange(IShape range)
        {
            return this.Where(item => range.Intersects(_getBox(item)));
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        void ICollection<T>.Add(T item)
        {
            if (!Add(item))
                throw new ArgumentException();
        }

        ISpace ISpace.GetPartition(Vector2 position)
        {
            return ContainsPoint(position) ? this : null;
        }

        ISpace ISpace.GetBestPartition(Vector2 position)
        {
            return ContainsPoint(position) ? this : null;
        }

        ISpace<T> ISpace<T>.GetPartition(Vector2 position)
        {
            return ContainsPoint(position) ? this : null;
        }

        ISpace<T> ISpace<T>.GetBestPartition(Vector2 position)
        {
            return ContainsPoint(position) ? this : null;
        }

        IEnumerable<ISpace> ISpace.GetAllPartitionsInRange(IRectangle range)
        {
            if (BoundingBox.Intersects(range))
                yield return this;
        }

        IEnumerable<ISpace<T>> ISpace<T>.GetAllPartitionsInRange(IRectangle range)
        {
            if (BoundingBox.Intersects(range))
                yield return this;
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}