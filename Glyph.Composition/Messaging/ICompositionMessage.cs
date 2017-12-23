using Glyph.Messaging;

namespace Glyph.Composition.Messaging
{
    public interface ICompositionMessage<out T> : IMessage
    {
        T Instance { get; }
    }
}