namespace Glyph.Messaging
{
    public interface IInterpreter<in TMessage>
        where TMessage : Message
    {
        void Interpret(TMessage message);
    }
}
