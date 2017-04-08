using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public abstract class WriteableSpaceBase<T> : IWriteableSpace<T>
    {
        protected readonly Func<T, Vector2> GetPoint;
        protected readonly Func<T, TopLeftRectangle> GetBox;
        private readonly IReadOnlyCollection<IWriteableSpace<T>> _readOnlyPartitions;
        private readonly List<IWriteableSpace<T>> _partitions;
        private readonly IPartitioner _partitioner;
        private bool _changing;

        protected abstract ICollection<T> Items { get; }
        public WriteableSpaceBase<T> Parent { get; }
        public TopLeftRectangle BoundingBox => _partitioner.BoundingBox;
        public int Count => Items.Count;
        public IEnumerable<Vector2> Points => Items.Select(GetPoint);
        public IEnumerable<TopLeftRectangle> Boxes => Items.Select(GetBox);
        public IEnumerable<IWriteableSpace<T>> Partitions => _readOnlyPartitions;

        IEnumerable<ISpace<T>> ISpace<T>.Partitions => Partitions;
        IEnumerable<ISpace> ISpace.Partitions => Partitions;
        bool ICollection<T>.IsReadOnly => false;

        protected WriteableSpaceBase(WriteableSpaceBase<T> parent, Func<T, Vector2> getPoint, Func<T, TopLeftRectangle> getBox, IPartitioner partitioner = null)
        {
            Parent = parent;
            GetPoint = getPoint;
            GetBox = getBox;

            _partitions = new List<IWriteableSpace<T>>();
            _readOnlyPartitions = _partitions.AsReadOnly();
            _partitioner = partitioner;
        }

        public bool Add(T item)
        {
            _changing = true;

            if (_partitioner != null && !_partitioner.Intersects(GetBox(item)))
                return false;

            AddToParents(item);
            Items.Add(item);

            if (_partitioner != null)
            {
                if (_partitions.Count == 0)
                {
                    if (Count > _partitioner.Capacity)
                    {
                        _partitions.AddRange(_partitioner.Subdivide().Select(CreatePartition));
                        if (Items.Any(itemToMove => !_partitions.Any(child => child.Add(itemToMove))))
                            throw new InvalidOperationException();
                    }
                }
                else if (!_partitions.Any(child => child.Add(item)))
                    throw new InvalidOperationException();
            }

            _changing = false;
            return true;
        }

        private void AddToParents(T item)
        {
            if (Parent == null || Parent._changing)
                return;

            Parent.Items.Add(item);
            Parent.AddToParents(item);
        }

        protected abstract IWriteableSpace<T> CreatePartition(IPartitioner partitioner);

        public bool Remove(T item)
        {
            _changing = true;

            if (!Items.Remove(item))
                return false;

            RemoveFromParents(item);

            if (!_partitions.Any(child => child.Remove(item)))
                throw new InvalidOperationException();

            _changing = false;
            return true;
        }

        private void RemoveFromParents(T item)
        {
            if (Parent == null || Parent._changing)
                return;

            if (!Parent.Items.Remove(item))
                throw new InvalidOperationException();

            Parent.RemoveFromParents(item);
        }

        public void Clear()
        {
            RemoveAllFromParents(Items);
            Items.Clear();
            _partitions.Clear();
        }

        private void RemoveAllFromParents(ICollection<T> items)
        {
            if (Parent == null || Parent._changing)
                return;

            if (items.Any(item => !Parent.Items.Remove(item)))
                throw new InvalidOperationException();

            Parent.RemoveAllFromParents(items);
        }

        public bool Contains(T item)
        {
            return Items.Contains(item);
        }

        public bool ContainsPoint(Vector2 point)
        {
            return _partitioner == null || _partitioner.ContainsPoint(point);
        }

        public IWriteableSpace<T> GetPartition(Vector2 position)
        {
            return _partitions.FirstOrDefault(x => x.ContainsPoint(position));
        }

        public IWriteableSpace<T> GetBestPartition(Vector2 position)
        {
            if (_partitioner == null)
                return null;

            if (_partitions.Count == 0)
                return _partitioner.ContainsPoint(position) ? this : null;

            return _partitions.Select(partition => partition.GetBestPartition(position)).FirstOrDefault(bestPartition => bestPartition != null);
        }

        public IEnumerable<IWriteableSpace<T>> GetAllPartitionsInRange(TopLeftRectangle range)
        {
            if (_partitioner != null && !_partitioner.Intersects(range))
                yield break;

            if (_partitions.Count == 0)
            {
                yield return this;
                yield break;
            }

            foreach (IWriteableSpace<T> partition in _partitions)
                foreach (IWriteableSpace<T> result in partition.GetAllPartitionsInRange(range))
                    yield return result;
        }

        public IEnumerable<Vector2> GetAllPointsInRange(IShape range)
        {
            return GetAllItemsInRange(range).Select(GetPoint);
        }

        public IEnumerable<TopLeftRectangle> GetAllBoxesInRange(IShape range)
        {
            return GetAllItemsInRange(range).Select(GetBox);
        }

        public IEnumerable<T> GetAllItemsInRange(IShape range)
        {
            if (Items.Count == 0 || _partitioner != null && !_partitioner.Intersects(range))
                yield break;

            if (_partitions.Count == 0)
            {
                foreach (T item in Items.Where(x => range.Intersects(GetBox(x))))
                    yield return item;
                yield break;
            }

            foreach (ISpace<T> partition in Partitions)
                foreach (T item in partition.GetAllItemsInRange(range))
                    yield return item;
        }

        void ICollection<T>.Add(T item)
        {
            if (!Add(item))
                throw new ArgumentException();
        }

        ISpace<T> ISpace<T>.GetPartition(Vector2 position)
        {
            return GetPartition(position);
        }

        ISpace<T> ISpace<T>.GetBestPartition(Vector2 position)
        {
            return GetBestPartition(position);
        }

        IEnumerable<ISpace<T>> ISpace<T>.GetAllPartitionsInRange(TopLeftRectangle range)
        {
            return GetAllPartitionsInRange(range);
        }

        ISpace ISpace.GetPartition(Vector2 position)
        {
            return GetPartition(position);
        }

        ISpace ISpace.GetBestPartition(Vector2 position)
        {
            return GetBestPartition(position);
        }

        IEnumerable<ISpace> ISpace.GetAllPartitionsInRange(TopLeftRectangle range)
        {
            return GetAllPartitionsInRange(range);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}