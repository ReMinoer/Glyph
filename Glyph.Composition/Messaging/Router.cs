using System.Collections.Generic;

namespace Glyph.Composition.Messaging
{
    public class Router<TMessage> : IRouter<TMessage>
        where TMessage : Message
    {
        private readonly List<IInterpreter<TMessage>> _interpreters;
        public IReadOnlyList<IInterpreter<TMessage>> Interpreters { get; private set; }

        public Router()
        {
            _interpreters = new List<IInterpreter<TMessage>>();
            Interpreters = _interpreters.AsReadOnly();
        }

        public void Send(TMessage message)
        {
            foreach (IInterpreter<TMessage> interpreter in Interpreters)
                interpreter.OnMessage(message);
        }

        public void Register(IInterpreter<TMessage> interpreter)
        {
            _interpreters.Add(interpreter);
        }

        public void Unregister(IInterpreter<TMessage> interpreter)
        {
            _interpreters.Remove(interpreter);
        }
    }
}