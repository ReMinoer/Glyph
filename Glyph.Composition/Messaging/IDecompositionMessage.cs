using Glyph.Messaging;

namespace Glyph.Composition.Messaging
{
    public interface IDecompositionMessage<out T> : IMessage
    {
        T Instance { get; }
    }
}