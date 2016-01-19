using System.Collections.Generic;

namespace Glyph.Messaging
{
    public abstract class RouterBase<TMessage> : IRouter<TMessage>
        where TMessage : Message
    {
        private readonly List<IInterpreter<TMessage>> _interpreters;
        public IReadOnlyList<IInterpreter<TMessage>> Interpreters { get; private set; }

        protected RouterBase()
        {
            _interpreters = new List<IInterpreter<TMessage>>();
            Interpreters = _interpreters.AsReadOnly();
        }

        public abstract void Send(TMessage message);

        public virtual void Register(IInterpreter<TMessage> interpreter)
        {
            _interpreters.Add(interpreter);
        }

        public virtual void Unregister(IInterpreter<TMessage> interpreter)
        {
            _interpreters.Remove(interpreter);
        }
    }
}