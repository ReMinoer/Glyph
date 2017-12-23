using Glyph.Messaging;

namespace Glyph.Composition.Messaging
{
    public class DecompositionMessage<T> : Message, IDecompositionMessage<T>
    {
        public T Instance { get; }

        public DecompositionMessage(T instance)
        {
            Instance = instance;
        }
    }
}