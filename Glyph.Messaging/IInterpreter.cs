namespace Glyph.Messaging
{
    public interface IInterpreter
    {
    }
    
    public interface IInterpreter<in TMessage> : IInterpreter
        where TMessage : IMessage
    {
        void Interpret(TMessage message);
    }
}
