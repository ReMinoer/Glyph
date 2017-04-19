using System;
using System.Collections;
using System.Collections.Generic;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Composition.Messaging;
using Glyph.Messaging;

namespace Glyph.Core.Tracking
{
    public class MessagingTracker<T> : GlyphContainer, IMessagingCollection<T>
        where T : class
    {
        private readonly List<T> _tracker;
        private readonly List<T> _newInstances;
        public IReadOnlyCollection<T> NewInstances { get; }
        public Predicate<T> Filter { get; set; }

        public event Action<T> Registered;
        public event Action<T> Unregistered;

        public MessagingTracker(IRouter<InstantiatingMessage<T>> instantiatingRouter, IRouter<DisposingMessage<T>> disposingRouter)
        {
            _tracker = new List<T>();
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
            T instance = message.Instance;
            if (Filter != null && !Filter(message.Instance))
                return;

            _tracker.Add(instance);
            _newInstances.Add(instance);
            Registered?.Invoke(instance);
        }

        void IInterpreter<DisposingMessage<T>>.Interpret(DisposingMessage<T> message)
        {
            T instance = message.Instance;
            if (!_tracker.Remove(instance))
                return;

            _newInstances.Remove(instance);
            Unregistered?.Invoke(instance);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _tracker.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}