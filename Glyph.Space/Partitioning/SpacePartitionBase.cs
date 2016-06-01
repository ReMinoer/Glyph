using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Space.Partitioning
{
    public abstract class SpacePartitionBase<T> : ISpace<T>
    {
        protected readonly Func<T, Vector2> GetPoint;
        protected readonly Func<T, IRectangle> GetBox;
        private readonly List<T> _items;
        private readonly IReadOnlyCollection<ISpace<T>> _readOnlyPartitions;
        private List<ISpace<T>> _partitions;
        public ISpace<T> Parent { get; private set; }
        public int Capacity { get; private set; }

        public int Count
        {
            get { return _items.Count; }
        }

        public IEnumerable<Vector2> Points
        {
            get { return _items.Select(GetPoint); }
        }

        public IEnumerable<IRectangle> Boxes
        {
            get { return _items.Select(GetBox); }
        }

        public IEnumerable<ISpace<T>> Partitions
        {
            get { return _readOnlyPartitions; }
        }

        IEnumerable<ISpace> ISpace.Partitions
        {
            get { return Partitions; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }
        
        public abstract IRectangle BoundingBox { get; }

        protected SpacePartitionBase(ISpace<T> parent, Func<T, Vector2> getPoint, int capacity)
            : this(parent, getPoint, x => new CenteredRectangle(getPoint(x), 0, 0), capacity)
        {
        }

        protected SpacePartitionBase(ISpace<T> parent, Func<T, IRectangle> getBox, int capacity)
            : this(parent, x => getBox(x).Center, getBox, capacity)
        {
        }

        protected SpacePartitionBase(ISpace<T> parent, Func<T, Vector2> getPoint, Func<T, IRectangle> getBox, int capacity)
        {
            Parent = parent;
            GetPoint = getPoint;
            GetBox = getBox;
            Capacity = capacity;

            _items = new List<T>();
            _partitions = new List<ISpace<T>>();
            _readOnlyPartitions = _partitions.AsReadOnly();
        }

        public abstract bool ContainsPoint(Vector2 point);
        protected abstract bool Intersects(IShape range);
        protected abstract IEnumerable<ISpace<T>> Subdivide();

        public bool Add(T item)
        {
            if (!Intersects(GetBox(item)))
                return false;

            if (!Partitions.Any())
            {
                if (Count > Capacity)
                    _items.Add(item);
                else
                {
                    _partitions = Subdivide().ToList();

                    if (this.Any(itemToMove => !Partitions.Any(x => x.Add(itemToMove))))
                        throw new InvalidOperationException();
                }
                return true;
            }

            if (Partitions.Any(partition => partition.Add(item)))
                return true;

            throw new InvalidOperationException();
        }

        public bool Remove(T item)
        {
            return _items.Remove(item) && Partitions.Any(child => child.Remove(item));
        }

        public void Clear()
        {
            _items.Clear();
            _partitions.Clear();
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public ISpace<T> GetPartition(Vector2 position)
        {
            return Partitions.First(x => ContainsPoint(position));
        }

        public ISpace<T> GetBestPartition(Vector2 position)
        {
            if (!this.Any())
                return ContainsPoint(position) ? this : null;

            foreach (ISpace<T> partition in Partitions)
            {
                ISpace<T> bestPartition = partition.GetBestPartition(position);
                if (bestPartition != null)
                    return bestPartition;
            }

            return null;
        }

        public IEnumerable<ISpace<T>> GetAllPartitionsInRange(IRectangle range)
        {
            if (!Intersects(range))
                yield break;

            if (!Partitions.Any())
            {
                yield return this;
                yield break;
            }

            foreach (ISpace<T> partition in Partitions)
                foreach (ISpace<T> result in partition.GetAllPartitionsInRange(range))
                    yield return result;
        }

        public IEnumerable<Vector2> GetAllPointsInRange(IShape range)
        {
            return GetAllItemsInRange(range).Select(GetPoint);
        }

        public IEnumerable<IRectangle> GetAllBoxesInRange(IShape range)
        {
            return GetAllItemsInRange(range).Select(GetBox);
        }

        public IEnumerable<T> GetAllItemsInRange(IShape range)
        {
            if (!Intersects(range))
                yield break;

            foreach (T item in this)
                if (range.Intersects(GetBox(item)))
                    yield return item;

            if (this.Any())
                foreach (ISpace<T> partition in Partitions)
                    foreach (T item in partition.GetAllItemsInRange(range))
                        yield return item;
        }

        void ICollection<T>.Add(T item)
        {
            if (!Add(item))
                throw new ArgumentException();
        }

        ISpace ISpace.GetPartition(Vector2 position)
        {
            return GetPartition(position);
        }

        ISpace ISpace.GetBestPartition(Vector2 position)
        {
            return GetBestPartition(position);
        }

        IEnumerable<ISpace> ISpace.GetAllPartitionsInRange(IRectangle range)
        {
            return GetAllPartitionsInRange(range);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}