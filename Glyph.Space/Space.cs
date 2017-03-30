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
        private readonly Func<T, TopLeftRectangle> _getBox;

        public IEnumerable<Vector2> Points
        {
            get { return _items.Select(x => _getBox(x).Center); }
        }

        public IEnumerable<TopLeftRectangle> Boxes
        {
            get { return _items.Select(x => _getBox(x)); }
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public TopLeftRectangle BoundingBox
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

        public Space(Func<T, TopLeftRectangle> getBox)
            : this(x => getBox(x).Center, getBox)
        {
        }

        public Space(Func<T, Vector2> getPoint, Func<T, TopLeftRectangle> getBox)
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
            return _items.Select(x => _getPoint(x)).Where(range.ContainsPoint);
        }

        public IEnumerable<TopLeftRectangle> GetAllBoxesInRange(IShape range)
        {
            return _items.Select(item => _getBox(item)).Where(range.Intersects);
        }

        public IEnumerable<T> GetAllItemsInRange(IShape range)
        {
            return _items.Where(item => range.Intersects(_getBox(item)));
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

        IEnumerable<ISpace> ISpace.GetAllPartitionsInRange(TopLeftRectangle range)
        {
            if (BoundingBox.Intersects(range))
                yield return this;
        }

        IEnumerable<ISpace<T>> ISpace<T>.GetAllPartitionsInRange(TopLeftRectangle range)
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