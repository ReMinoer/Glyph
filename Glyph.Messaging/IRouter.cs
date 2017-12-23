using Diese.Collections;

namespace Glyph.Messaging
{
    public interface IRouter : ITracker<IInterpreter>
    {
        void Send(IMessage message);
    }
}