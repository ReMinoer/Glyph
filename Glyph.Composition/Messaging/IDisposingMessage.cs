using Glyph.Messaging;

namespace Glyph.Composition.Messaging
{
    public interface IDisposingMessage<out T> : IMessage
    {
        T Instance { get; }
    }
}