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
    public class MessagingSpace<T> : GlyphContainer, IInterpreter<InstantiatingMessage<T>>, IInterpreter<DisposingMessage<T>>, ISpace<T>
        where T : class, IBoxedComponent
    {
        private readonly Space<T> _space;
        private readonly List<T> _newInstances;
        public ReadOnlyCollection<T> NewInstances { get; }

        public TopLeftRectangle BoundingBox => _space.BoundingBox;
        public IEnumerable<Vector2> Points => _space.Points;
        public IEnumerable<TopLeftRectangle> Boxes => _space.Boxes;
        public IEnumerable<ISpace<T>> Partitions => ((ISpace<T>)_space).Partitions;
        IEnumerable<ISpace> ISpace.Partitions => ((ISpace)_space).Partitions;

        public event Action<T> Registered;
        public event Action<T> Unregistered;

        public MessagingSpace(IRouter<InstantiatingMessage<T>> instantiatingRouter,
                              IRouter<DisposingMessage<T>> disposingRouter,
                              Func<T, Vector2> getPoint,
                              IPartitioner partitioner = null)
            : this(instantiatingRouter, disposingRouter, getPoint, x => new CenteredRectangle(getPoint(x), 0, 0), partitioner)
        {

        }
        public MessagingSpace(IRouter<InstantiatingMessage<T>> instantiatingRouter,
                              IRouter<DisposingMessage<T>> disposingRouter,
                              Func<T, TopLeftRectangle> getBox,
                              IPartitioner partitioner = null)
            : this(instantiatingRouter, disposingRouter, x => getBox(x).Center, getBox, partitioner)
        {

        }

        public MessagingSpace(IRouter<InstantiatingMessage<T>> instantiatingRouter,
                              IRouter<DisposingMessage<T>> disposingRouter,
                              Func<T, Vector2> getPoint,
                              Func<T, TopLeftRectangle> getBox,
                              IPartitioner partitioner = null)
        {
            _space = new Space<T>(getPoint, getBox, partitioner);
            _newInstances = new List<T>();
            NewInstances = new ReadOnlyCollection<T>(_newInstances);

            Components.Add(new Receiver<InstantiatingMessage<T>>(instantiatingRouter, this));
            Components.Add(new Receiver<DisposingMessage<T>>(disposingRouter, this));
        }

        public void CleanNewInstances()
        {
            _newInstances.Clear();
        }

        void IInterpreter<InstantiatingMessage<T>>.Interpret(InstantiatingMessage<T> message)
        {
            _space.Add(message.Instance);
            _newInstances.Add(message.Instance);

            Registered?.Invoke(message.Instance);
        }

        void IInterpreter<DisposingMessage<T>>.Interpret(DisposingMessage<T> message)
        {
            _space.Remove(message.Instance);
            _newInstances.Remove(message.Instance);

            Unregistered?.Invoke(message.Instance);
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