using System;
using System.Collections;
using System.Collections.Generic;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Composition.Messaging;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Messaging;
using Glyph.Space;
using Microsoft.Xna.Framework;

namespace Glyph.Core.Tracking
{
    public class MessagingSpace<T> : GlyphContainer, ISpace<T>, IMessagingCollection<T>
        where T : class, IBoxedComponent
    {
        private readonly Space<T> _space;
        private readonly List<T> _newInstances;
        public IReadOnlyCollection<T> NewInstances { get; }
        public Predicate<T> Filter { get; set; }

        public bool IsVoid => _space.IsVoid;
        public TopLeftRectangle BoundingBox => _space.BoundingBox;
        public IEnumerable<Vector2> Points => _space.Points;
        public IEnumerable<TopLeftRectangle> Boxes => _space.Boxes;
        public IEnumerable<ISpace<T>> Partitions => ((ISpace<T>)_space).Partitions;
        IEnumerable<ISpace> ISpace.Partitions => ((ISpace)_space).Partitions;

        public event Action<T> Registered;
        public event Action<T> Unregistered;

        public MessagingSpace(IRouter router, Func<T, Vector2> getPoint, IPartitioner partitioner = null)
            : this(router, getPoint, x => new CenteredRectangle(getPoint(x), 0, 0), partitioner)
        {
        }
        
        public MessagingSpace(IRouter router, Func<T, TopLeftRectangle> getBox, IPartitioner partitioner = null)
            : this(router, x => getBox(x).Center, getBox, partitioner)
        {
        }

        public MessagingSpace(IRouter router, Func<T, Vector2> getPoint, Func<T, TopLeftRectangle> getBox, IPartitioner partitioner = null)
        {
            _space = new Space<T>(getPoint, getBox, partitioner);
            _newInstances = new List<T>();
            NewInstances = new ReadOnlyCollection<T>(_newInstances);

            Components.Add(new Receiver<IInstantiatingMessage<T>>(router, this));
            Components.Add(new Receiver<IDisposingMessage<T>>(router, this));
        }

        public void CleanNewInstances()
        {
            _newInstances.Clear();
        }

        void IInterpreter<IInstantiatingMessage<T>>.Interpret(IInstantiatingMessage<T> message)
        {
            T instance = message.Instance;
            if (Filter != null && !Filter(message.Instance))
                return;

            _space.Add(instance);
            _newInstances.Add(instance);
            Registered?.Invoke(instance);
        }

        void IInterpreter<IDisposingMessage<T>>.Interpret(IDisposingMessage<T> message)
        {
            T instance = message.Instance;
            if (!_space.Remove(instance))
                return;

            _newInstances.Remove(instance);
            Unregistered?.Invoke(instance);
        }

        public bool ContainsPoint(Vector2 point)
        {
            return _space.ContainsPoint(point);
        }

        public ISpace<T> GetPartition(Vector2 position)
        {
            return ((ISpace<T>)_space).GetPartition(position);
        }

        public ISpace<T> GetBestPartition(Vector2 position)
        {
            return ((ISpace<T>)_space).GetBestPartition(position);
        }

        public IEnumerable<ISpace<T>> GetAllPartitionsInRange(TopLeftRectangle range)
        {
            return ((ISpace<T>)_space).GetAllPartitionsInRange(range);
        }

        public IEnumerable<T> GetAllItemsInRange(IShape range)
        {
            return _space.GetAllItemsInRange(range);
        }

        public IEnumerable<Vector2> GetAllPointsInRange(IShape range)
        {
            return _space.GetAllPointsInRange(range);
        }

        public IEnumerable<TopLeftRectangle> GetAllBoxesInRange(IShape range)
        {
            return _space.GetAllBoxesInRange(range);
        }

        ISpace ISpace.GetPartition(Vector2 position)
        {
            return ((ISpace)_space).GetPartition(position);
        }

        ISpace ISpace.GetBestPartition(Vector2 position)
        {
            return ((ISpace)_space).GetBestPartition(position);
        }

        IEnumerable<ISpace> ISpace.GetAllPartitionsInRange(TopLeftRectangle range)
        {
            return ((ISpace)_space).GetAllPartitionsInRange(range);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _space.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_space).GetEnumerator();
        }
    }
}