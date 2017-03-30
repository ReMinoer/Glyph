using System;
using System.Collections;
using System.Collections.Generic;
using Diese.Collections.Trackers;
using Glyph.Composition;
using Glyph.Composition.Messaging;
using Glyph.Messaging;

namespace Glyph.Core.Tracking
{
    public class MessagingTracker<T> : GlyphContainer, IInterpreter<InstantiatingMessage<T>>, IInterpreter<DisposingMessage<T>>, IEnumerable<T>
        where T : class
    {
        private readonly Tracker<T> _tracker;
        private readonly Tracker<T> _newInstances;
        public ReadOnlyTracker<T> NewInstances { get; private set; }

        public event Action<T> Registered;
        public event Action<T> Unregistered;

        public MessagingTracker(IRouter<InstantiatingMessage<T>> instantiatingRouter, IRouter<DisposingMessage<T>> disposingRouter)
        {
            _tracker = new Tracker<T>();
            _newInstances = new Tracker<T>();
            NewInstances = new ReadOnlyTracker<T>(_newInstances);

            Components.Add(new Receiver<InstantiatingMessage<T>>(instantiatingRouter, this));
            Components.Add(new Receiver<DisposingMessage<T>>(disposingRouter, this));
        }

        public void CleanNewInstances()
        {
            _newInstances.Clear();
        }

        void IInterpreter<InstantiatingMessage<T>>.Interpret(InstantiatingMessage<T> message)
        {
            _tracker.Register(message.Instance);
            _newInstances.Register(message.Instance);

            Registered?.Invoke(message.Instance);
        }

        void IInterpreter<DisposingMessage<T>>.Interpret(DisposingMessage<T> message)
        {
            _tracker.Unregister(message.Instance);
            _newInstances.Unregister(message.Instance);

            Unregistered?.Invoke(message.Instance);
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