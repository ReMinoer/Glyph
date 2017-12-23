using Glyph.Messaging;

namespace Glyph.Composition.Messaging
{
    public interface IInstantiatingMessage<out T> : IMessage
    {
        T Instance { get; }
    }
}