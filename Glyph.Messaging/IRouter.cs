using Diese.Collections.Trackers;

namespace Glyph.Messaging
{
    public interface IRouter<TMessage> : ITracker<IInterpreter<TMessage>>
        where TMessage : Message
    {
        void Send(TMessage message);
    }
}