namespace Glyph.Messaging
{
    public class InstantiatingMessage<T> : Message
    {
        public T Instance { get; private set; }

        public InstantiatingMessage(T instance)
        {
            Instance = instance;
        }
    }
}