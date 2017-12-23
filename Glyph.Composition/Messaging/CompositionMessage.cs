using Glyph.Messaging;

namespace Glyph.Composition.Messaging
{
    public class CompositionMessage<T> : Message, ICompositionMessage<T>
    {
        public T Instance { get; }

        public CompositionMessage(T instance)
        {
            Instance = instance;
        }
    }
}