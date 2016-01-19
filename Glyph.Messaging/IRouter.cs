namespace Glyph.Messaging
{
    public interface IRouter<TMessage>
        where TMessage : Message
    {
        void Send(TMessage message);
        void Register(IInterpreter<TMessage> interpreter);
        void Unregister(IInterpreter<TMessage> interpreter);
    }
}