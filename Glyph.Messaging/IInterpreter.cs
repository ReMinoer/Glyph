namespace Glyph.Messaging
{
    public interface IInterpreter<in TMessage>
        where TMessage : Message
    {
        void OnMessage(TMessage message);
    }
}
