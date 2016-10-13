namespace Glyph.Messaging
{
    public interface ILocalInterpreter<in TMessage> : IInterpreter<TMessage>
        where TMessage : Message
    {
        
    }
}