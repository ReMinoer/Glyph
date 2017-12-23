using Glyph.Messaging;

namespace Glyph.Composition.Messaging
{
    public class DisposingMessage<T> : Message, IDisposingMessage<T>
    {
        public T Instance { get; private set; }

        public DisposingMessage(T instance)
        {
            Instance = instance;
        }
    }
}