using System;
using System.Collections.Generic;
using Diese.Collections;
using Diese.Collections.Trackers;
using Glyph.Composition.Messaging;
using Glyph.Messaging;

namespace Glyph.Composition.Tracking
{
    public class MessagingTracker<T> : GlyphContainer, IInterpreter<InstantiatingMessage<T>>, IInterpreter<DisposingMessage<T>>, IEnumerable<T>
        where T : class
    {
        private readonly Tracker<T> _tracker;

        public event Action<T> Registered;
        public event Action<T> Unregistered;

        public MessagingTracker(IRouter<InstantiatingMessage<T>> instantiatingRouter, IRouter<DisposingMessage<T>> disposingRouter)
        {
            _tracker = new Tracker<T>();

            Components.Add(new Receiver<InstantiatingMessage<T>>(instantiatingRouter, this));
            Components.Add(new Receiver<DisposingMessage<T>>(disposingRouter, this));
        }

        void IInterpreter<InstantiatingMessage<T>>.Interpret(InstantiatingMessage<T> message)
        {
            _tracker.Register(message.Instance);

            if (Registered != null)
                Registered.Invoke(message.Instance);
        }

        void IInterpreter<DisposingMessage<T>>.Interpret(DisposingMessage<T> message)
        {
            _tracker.Unregister(message.Instance);

            if (Unregistered != null)
                Unregistered.Invoke(message.Instance);
        }

        new public IEnumerator<T> GetEnumerator()
        {
            return _tracker.GetEnumerator();
        }
    }
}