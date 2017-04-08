using Diese.Collections;

namespace Glyph.Messaging
{
    public interface IRouter<TMessage> : ITracker<IInterpreter<TMessage>>
        where TMessage : Message
    {
        void Send(TMessage message);
    }
}