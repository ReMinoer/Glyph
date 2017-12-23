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

        public MessagingTracker(IRouter router)
        {
            _tracker = new List<T>();
            _newInstances = new List<T>();
            NewInstances = new ReadOnlyCollection<T>(_newInstances);

            Components.Add(new Receiver<InstantiatingMessage<T>>(router, this));
            Components.Add(new Receiver<DisposingMessage<T>>(router, this));
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

            _tracker.Add(instance);
            _newInstances.Add(instance);
            Registered?.Invoke(instance);
        }

        void IInterpreter<IDisposingMessage<T>>.Interpret(IDisposingMessage<T> message)
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