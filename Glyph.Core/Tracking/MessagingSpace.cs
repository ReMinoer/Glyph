using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Diese.Collections;
using Glyph.Composition.Messaging;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Messaging;
using Glyph.Space;
using Microsoft.Xna.Framework;
using Stave;

namespace Glyph.Core.Tracking
{
    public class MessagingSpace<T> : ISpace<T>, IMessagingCollection<T>, IDisposable
        where T : class, IBoxedComponent
    {
        private readonly ISubscribableRouter _router;
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

        public MessagingSpace(ISubscribableRouter router, Func<T, Vector2> getPoint, IPartitioner partitioner = null)
            : this(router, getPoint, x => new CenteredRectangle(getPoint(x), 0, 0), partitioner)
        {
        }
        
        public MessagingSpace(ISubscribableRouter router, Func<T, TopLeftRectangle> getBox, IPartitioner partitioner = null)
            : this(router, x => getBox(x).Center, getBox, partitioner)
        {
        }

        public MessagingSpace(ISubscribableRouter router, Func<T, Vector2> getPoint, Func<T, TopLeftRectangle> getBox, IPartitioner partitioner = null)
        {
            _router = router;
            _space = new Space<T>(getPoint, getBox, partitioner);
            _newInstances = new List<T>();
            NewInstances = new ReadOnlyCollection<T>(_newInstances);

            _router.Add<ICompositionMessage<T>>(Interpret);
            _router.Add<IDecompositionMessage<T>>(Interpret);
        }

        public void CleanNewInstances()
        {
            _newInstances.Clear();
        }

        void IInterpreter<ICompositionMessage<T>>.Interpret(ICompositionMessage<T> message) => Interpret(message);
        void IInterpreter<IDecompositionMessage<T>>.Interpret(IDecompositionMessage<T> message) => Interpret(message);

        private void Interpret(ICompositionMessage<T> message)
        {
            Add(message.Instance);
            foreach (T child in message.Instance.ChildrenQueue().OfType<T>())
                Add(child);
        }

        private void Add(T instance)
        {
            if (Filter != null && !Filter(instance))
                return;

            _space.Add(instance);
            _newInstances.Add(instance);
            Registered?.Invoke(instance);
        }

        private void Interpret(IDecompositionMessage<T> message)
        {
            Remove(message.Instance);
            foreach (T child in message.Instance.ChildrenQueue().OfType<T>())
                Remove(child);
        }

        private void Remove(T instance)
        {
            if (!_space.Remove(instance))
                return;

            _newInstances.Remove(instance);
            Unregistered?.Invoke(instance);
        }

        public bool ContainsPoint(Vector2 point) => _space.ContainsPoint(point);
        public bool Intersects(Segment segment) => _space.Intersects(segment);
        public bool Intersects<TShape>(TShape edgedShape) where TShape : IEdgedShape => _space.Intersects(edgedShape);
        public bool Intersects(Circle circle) => _space.Intersects(circle);
        public ISpace<T> GetPartition(Vector2 position) => ((ISpace<T>)_space).GetPartition(position);
        public ISpace<T> GetBestPartition(Vector2 position) => ((ISpace<T>)_space).GetBestPartition(position);
        public IEnumerable<ISpace<T>> GetAllPartitionsInRange(TopLeftRectangle range) => ((ISpace<T>)_space).GetAllPartitionsInRange(range);
        public IEnumerable<T> GetAllItemsInRange(IShape range) => _space.GetAllItemsInRange(range);
        public IEnumerable<Vector2> GetAllPointsInRange(IShape range) => _space.GetAllPointsInRange(range);
        public IEnumerable<TopLeftRectangle> GetAllBoxesInRange(IShape range) => _space.GetAllBoxesInRange(range);
        ISpace ISpace.GetPartition(Vector2 position) => ((ISpace)_space).GetPartition(position);
        ISpace ISpace.GetBestPartition(Vector2 position) => ((ISpace)_space).GetBestPartition(position);
        IEnumerable<ISpace> ISpace.GetAllPartitionsInRange(TopLeftRectangle range) => ((ISpace)_space).GetAllPartitionsInRange(range);
        public IEnumerator<T> GetEnumerator() => _space.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_space).GetEnumerator();

        public void Dispose()
        {
            _router.Remove<ICompositionMessage<T>>(Interpret);
            _router.Remove<IDecompositionMessage<T>>(Interpret);
        }
    }
}