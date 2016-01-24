using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Space.Partitioning.Base
{
    public abstract class PartitionBase<T> : IPartition<T>
    {
        private readonly List<T> _items;
        private readonly IReadOnlyList<T> _readOnlyItems;
        protected readonly Func<T, Vector2> GetPosition;
        public IPartition<T> Parent { get; private set; }
        public int Capacity { get; private set; }

        public IEnumerable<T> Items
        {
            get { return _readOnlyItems; }
        }

        protected PartitionBase(Func<T, Vector2> getPosition, IPartition<T> parent, int capacity)
        {
            GetPosition = getPosition;
            Parent = parent;
            Capacity = capacity;

            _items = new List<T>();
            _readOnlyItems = _items.AsReadOnly();
        }

        public abstract bool ContainsPoint(Vector2 position);
        protected abstract bool Intersects(IShape range);
        protected abstract void Subdivide();

        public bool TryInsert(T item, int depth, int depthMax)
        {
            if (!ContainsPoint(GetPosition(item)))
                return false;

            if (AddItem(item, depth, depthMax))
                return true;

            if (!this.Any() && depth < depthMax)
            {
                Subdivide();

                foreach (T itemToMove in Items)
                    foreach (IPartition<T> partition in this)
                        if (partition.TryInsert(itemToMove, depth + 1, depthMax))
                            break;

                _items.Clear();
            }

            foreach (IPartition<T> node in this)
                if (node.TryInsert(item, depth + 1, depthMax))
                    return true;

            throw new InvalidOperationException();
        }

        public bool TryRemove(T item)
        {
            if (_items.Contains(item))
            {
                _items.Remove(item);
                return true;
            }

            foreach (IPartition<T> child in this)
                child.TryRemove(item);

            return false;
        }

        protected bool AddItem(T item, int depth, int depthMax)
        {
            if (Items.Count() < Capacity || depth == depthMax)
            {
                _items.Add(item);
                return true;
            }
            return false;
        }

        public IPartition<T> GetBestPartition(Vector2 position)
        {
            if (!this.Any())
                return ContainsPoint(position) ? this : null;

            foreach (IPartition<T> partition in this)
            {
                IPartition<T> bestPartition = partition.GetBestPartition(position);
                if (bestPartition != null)
                    return bestPartition;
            }

            return null;
        }

        public IEnumerable<T> GetAllPointsInRange(IShape range)
        {
            if (!Intersects(range))
                return Enumerable.Empty<T>();
            
            var result = new List<T>();

            foreach (T item in Items)
                if (range.ContainsPoint(GetPosition(item)))
                    result.Add(item);

            if (this.Any())
                foreach (IPartition<T> partition in this)
                    result.AddRange(partition.GetAllPointsInRange(range));

            return result;
        }

        public abstract IEnumerator<IPartition<T>> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}