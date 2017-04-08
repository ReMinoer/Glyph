using System;
using System.Collections;
using System.Collections.Generic;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Composition.Messaging;
using Glyph.Messaging;

namespace Glyph.Core.Tracking
{
    public class MessagingTracker<T> : GlyphContainer, IInterpreter<InstantiatingMessage<T>>, IInterpreter<DisposingMessage<T>>, IEnumerable<T>
        where T : class
    {
        private readonly List<T> _tracker;
        private readonly List<T> _newInstances;
        public ReadOnlyCollection<T> NewInstances { get; private set; }

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
            _tracker.Add(message.Instance);
            _newInstances.Add(message.Instance);

            Registered?.Invoke(message.Instance);
        }

        void IInterpreter<DisposingMessage<T>>.Interpret(DisposingMessage<T> message)
        {
            _tracker.Remove(message.Instance);
            _newInstances.Remove(message.Instance);

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