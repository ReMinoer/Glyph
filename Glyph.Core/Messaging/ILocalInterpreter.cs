using Glyph.Messaging;

namespace Glyph.Core.Messaging
{
    public interface ILocalInterpreter<in TMessage> : IInterpreter
        where TMessage : IMessage
    {
        void Interpret(TMessage message);
    }
}