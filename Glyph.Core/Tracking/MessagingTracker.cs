using System;
using System.Collections;
using System.Collections.Generic;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Composition.Messaging;
using Glyph.Messaging;

namespace Glyph.Core.Tracking
{
    public class MessagingTracker<T> : IMessagingCollection<T>
        where T : class, IGlyphComponent
    {
        private readonly ISubscribableRouter _router;
        private readonly List<T> _tracker;
        private readonly List<T> _newInstances;
        public IReadOnlyCollection<T> NewInstances { get; }
        public Predicate<T> Filter { get; set; }

        public event Action<T> Registered;
        public event Action<T> Unregistered;

        public MessagingTracker(ISubscribableRouter router)
        {
            _router = router;
            _tracker = new List<T>();
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
            T instance = message.Instance;
            if (Filter != null && !Filter(message.Instance))
                return;

            _tracker.Add(instance);
            _newInstances.Add(instance);
            Registered?.Invoke(instance);
        }

        private void Interpret(IDecompositionMessage<T> message)
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

        public void Dispose()
        {
            _router.Remove<ICompositionMessage<T>>(Interpret);
            _router.Remove<IDecompositionMessage<T>>(Interpret);
        }
    }
}