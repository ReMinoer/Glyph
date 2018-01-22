using Glyph.Messaging;

namespace Glyph.Composition.Messaging
{
    public interface ILocalInterpreter
    {
    }

    public interface ILocalInterpreter<in TMessage> : ILocalInterpreter
        where TMessage : IMessage
    {
        void Interpret(TMessage message);
    }
}