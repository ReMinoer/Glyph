using Glyph.Messaging;

namespace Glyph.Composition.Messaging
{
    public class InstantiatingMessage<T> : Message, IInstantiatingMessage<T>
    {
        public T Instance { get; }

        public InstantiatingMessage(T instance)
        {
            Instance = instance;
        }
    }
}