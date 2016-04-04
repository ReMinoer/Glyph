using Diese.Collections;

namespace Glyph.Messaging
{
    public abstract class RouterBase<TMessage> : Tracker<IInterpreter<TMessage>>, IRouter<TMessage>
        where TMessage : Message
    {
        public void Send(TMessage message)
        {
            foreach (IInterpreter<TMessage> interpreter in this)
            {
                if (message.IsHandled)
                    return;

                interpreter.Interpret(message);
            }
        }
    }
}